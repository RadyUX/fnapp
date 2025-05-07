extends Node2D

@export var spawn_limit := 5
@export var spawn_interval := 10.0
@export var customer_scene: PackedScene
@export var skins := ["npc_boy", "npc_girl"]
@onready var inventory = get_node("/root/Inventory")
var current_count := 0
@onready var spawn_timer: Timer = $spawnTimer

func _ready():
	randomize()
	spawn_timer.wait_time = spawn_interval
	spawn_timer.timeout.connect(_on_spawn_timer_timeout)
	DayAndNightCycleManager.TimeTick.connect(_on_time_tick)

	if not is_restaurant_closed():
		spawn_timer.start()

func _on_time_tick(day: int, hour: int, minute: int) -> void:
	if hour == 8 and minute == 0:
		print("☀️ Resto ouvert ! Timer relancé.")
		current_count = 0
		spawn_timer.start()

func is_restaurant_closed() -> bool:
	var time = DayAndNightCycleManager.Time
	var total_minutes = int(time / (TAU / (24 * 60))) # hardcode
	var hour = (total_minutes % 1440) / 60
	return hour >= 22 or hour < 8


func _on_spawn_timer_timeout():
	if current_count >= spawn_limit:
		spawn_timer.stop()
		return
	
	if is_restaurant_closed():
		print("⛔ Fermé, pas de spawn.")
		spawn_timer.stop()
		return

	spawn_customer_random()
	current_count += 1

func spawn_customer_random():
	var random_item: Item = GameManager.getRandomItem()
	var random_quantity: int= randi_range(1,3)
	
	var customer = customer_scene.instantiate()
	
	customer.init_customer(random_item, random_quantity)
	# skin aléatoire
	var skin = skins[randi() % skins.size()]
	customer.skin_name = skin
	# random request
	
	
	
	# POSITION : soit un point, soit random dans un Area2D
	if has_node("SpawnArea"):
		var area = $SpawnArea as Area2D
		var shape = area.get_node("CollisionShape2D").shape as RectangleShape2D
		var size = shape.extents * 2
		var rand_offset = Vector2(
			randf_range(-size.x/2, size.x/2),
			randf_range(-size.y/2, size.y/2)
		)
		customer.global_position = global_position + rand_offset
	else:
		customer.global_position = global_position

	get_parent().add_child(customer)
