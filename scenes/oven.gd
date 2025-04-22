extends StaticBody2D

@onready var interactable: Area2D = $interacteble
@onready var sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var pizza_crue := $pizzacru
@onready var pizza_cuite := $pizzacuit
@onready var detection_zone: Area2D = $DetectionZone

enum FourState { VIDE, AVEC_CRUE, EN_CUISSON, CUITE }
var state = FourState.VIDE

var pizzas_crues := []

func _ready():
	interactable.interact = _on_interact
	detection_zone.area_entered.connect(_on_pizza_detected)
	detection_zone.area_exited.connect(_on_pizza_gone)
	# Vérifie si tu as bien défini une méthode pour interagir ici
	# Sinon il faudra faire appel à _on_interact() depuis un signal externe (par exemple le joueur)ee
	pizza_cuite.visible = false

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
