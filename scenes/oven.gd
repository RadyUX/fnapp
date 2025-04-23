extends StaticBody2D

# --- Nœuds ---
@onready var interactable: Area2D = $interacteble
@onready var sprite_2d: AnimatedSprite2D = $AnimatedSprite2D
@onready var detection_zone: Area2D = $DetectionZone

# --- Scènes exportables ---
@export var pizza_crue_scene: PackedScene
@export var pizza_cuite_scene: PackedScene
@export var max_pizzas_on_counter := 2
# --- Offsets d'apparition ---
@export var pizza_spawn_position: Vector2 = Vector2(0, 0)
@export var pizza_cuite_spawn_offset := Vector2(0, -16)

# --- État du four ---
enum FourState { VIDE, AVEC_CRUE, EN_CUISSON }
var state = FourState.VIDE
var is_busy := false

# --- Liste des pizzas crues détectées ---
var pizzas_crues := []


func _ready():
	interactable.interact = _on_interact
	detection_zone.area_entered.connect(_on_pizza_detected)
	detection_zone.area_exited.connect(_on_pizza_gone)
	
	spawn_timer()
	spawn_pizza_crue()  # pour démarrer avec une première pizza


# --- Génére une pizza crue dynamiquement ---
func spawn_pizza_crue():
	var pizza_crue_scene = preload("res://scenes/pizzacru.tscn")
	if not pizza_crue_scene:
		print("❌ Pas de scène de pizza crue assignée")
		return

	var pizza_instance = pizza_crue_scene.instantiate()
	pizza_instance.global_position = detection_zone.global_position

	get_tree().current_scene.call_deferred("add_child", pizza_instance)

	# ✅ Forcer la détection après un léger délai
	await get_tree().create_timer(0.05).timeout  # attendre un mini instant que la scène s’instancie

	if detection_zone.overlaps_area(pizza_instance):
		_on_pizza_detected(pizza_instance)
	else:
		print("⚠️ Pizza non détectée malgré l'apparition")

# --- Appelé toutes les X secondes ---
func spawn_timer():
	await get_tree().create_timer(5).timeout
	spawn_pizza_crue()
	spawn_timer()  # boucle infinie


# --- Quand une pizza entre dans la zone ---
func _on_pizza_detected(area):
	if area.is_in_group("pizzacru") and area.is_ready_for_cuisson:
		pizzas_crues.append(area)
		print("🍕 Pizza détectée :", area.name)


# --- Quand une pizza quitte la zone ---
func _on_pizza_gone(area):
	if pizzas_crues.has(area):
		pizzas_crues.erase(area)


# --- Interaction du joueur ---
func _on_interact():
	if is_busy:
		return

	if pizzas_crues.size() == 0:
		print("❌ Pas de pizza à cuire")
		return

	is_busy = true
	state = FourState.EN_CUISSON

	var pizza = pizzas_crues[0]
	if is_instance_valid(pizza):
		pizza.queue_free()
	pizzas_crues.erase(pizza)

	sprite_2d.play("cook")
	print("🔥 Cuisson de la pizza...")

	await get_tree().create_timer(1.5).timeout

	sprite_2d.stop()
	print("✅ Pizza cuite prête")
	
	var pizza_cuite_scene = preload("res://scenes/pizzacuite.tscn")
	if pizza_cuite_scene:
		# Vérifier combien de pizzas sont déjà posées
		var pizzas_on_counter = get_tree().current_scene.get_children().filter(
			func(n): return n.is_in_group("pizzacuite")
		).size()

		if pizzas_on_counter >= max_pizzas_on_counter:
			print("🧍 Comptoir plein ! Retirez une pizza avant de continuer.")
			is_busy = false  # ✅ NE PAS OUBLIER !
			state = FourState.AVEC_CRUE if pizzas_crues.size() > 0 else FourState.VIDE
			return

	
	# Instance une nouvelle pizza cuite
	var pizza_instance = pizza_cuite_scene.instantiate()
	var shape = detection_zone.get_node("CollisionShape2D").shape

	if shape is RectangleShape2D:
		var extents = shape.extents
		var random_offset = Vector2(
			randf_range(-extents.x, extents.x),
			randf_range(-extents.y, extents.y)
		)
		pizza_instance.global_position = detection_zone.global_position + random_offset
	else:
		pizza_instance.global_position = detection_zone.global_position + pizza_cuite_spawn_offset

	get_tree().current_scene.call_deferred("add_child", pizza_instance)

	

	state = FourState.AVEC_CRUE if pizzas_crues.size() > 0 else FourState.VIDE
	is_busy = false
	print("🧺 Pizza cuite posée sur le comptoir")

func check_if_ready_after_space():
	print("📣 Un four a été notifié d’une place libre !")
	# ✅ Plus de cuisson automatique ici
	# Le joueur devra réappuyer pour cuire
