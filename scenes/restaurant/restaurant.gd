extends Node2D

@onready var spawn_timer = $spawnTimer
var customer_scene = preload("res://scenes/customers/Customer.tscn")
var skins = ["npc_boy", "npc_girl"]  # 👕 Liste des skins possibles

func spawn_customer_random():
	var customer = customer_scene.instantiate()
	
	# Tire aléatoirement "npc_boy" ou "npc_girl"
	var random_skin = skins[randi() % skins.size()]
	customer.skin_name = random_skin
	
	add_child(customer)

func _ready():
	randomize()
	spawn_timer.wait_time = 2 # 5 secondes
	spawn_timer.start()
	spawn_timer.timeout.connect(_on_spawn_timer_timeout)
	
func _on_spawn_timer_timeout():
	spawn_customer_random()
