extends StaticBody2D

@onready var interactable: Area2D = $interacteble
@onready var sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var pizza_crue := $pizzacru
@onready var pizza_cuite := $pizzacuit
@onready var detection_zone: Area2D = $DetectionZone
@export var pizza_crue_scene: PackedScene
@export var pizza_spawn_position: Vector2 = Vector2(0, 0)

enum FourState { VIDE, AVEC_CRUE, EN_CUISSON, CUITE }
var state = FourState.VIDE

var pizzas_crues := []

func spawn_pizza_crue():
	var pizza_crue_scene = preload("res://scenes/pizzacru.tscn")
	if not pizza_crue_scene:
		print("❌ Pas de scène de pizza assignée")
		return

	var pizza_instance = pizza_crue_scene.instantiate()
	pizza_instance.global_position = global_position + pizza_spawn_position
	get_tree().current_scene.add_child(pizza_instance)

	# ✅ FORCER LA DÉTECTION À LA MAIN :
	if pizza_instance.is_in_group("pizzacru") and pizza_instance.is_ready_for_cuisson:
		pizzas_crues.append(pizza_instance)
		print("🍕 Pizza ajoutée manuellement à la liste")
	else:
		print("❌ Pizza générée mais non détectable")

	print("🍕 Nouvelle pizza crue générée !")

	
func _ready():
	interactable.interact = _on_interact
	detection_zone.area_entered.connect(_on_pizza_detected)
	detection_zone.area_exited.connect(_on_pizza_gone)
	pizza_cuite.visible = false
	spawn_timer()
	spawn_pizza_crue()


func spawn_timer():
	await get_tree().create_timer(5).timeout
	spawn_pizza_crue()
	spawn_timer()  # relance en boucle

func _on_pizza_detected(area):
	if area.is_in_group("pizzacru") and area.is_ready_for_cuisson:
		pizzas_crues.append(area)
		print("🍕 Pizza ajoutée :", area.name)

func _on_pizza_gone(area):
	if pizzas_crues.has(area):
		pizzas_crues.erase(area)

func _on_interact():
	if pizzas_crues.size() == 0:
		print("❌ Pas de pizza à cuire")
		return

	while pizzas_crues.size() > 0:
		var pizza = pizzas_crues[0]
		pizza.queue_free()
		pizzas_crues.erase(pizza)

		sprite_2d.play("cook")
		print("🔥 Cuisson en cours...")

		await get_tree().create_timer(1.5).timeout

		sprite_2d.stop()
		pizza_cuite.visible = true
		print("✅ Pizza cuite prête")

		await get_tree().create_timer(1.0).timeout
		pizza_cuite.visible = false
		print("🧺 Pizza récupérée")
