extends Area2D

@export var is_interactable: bool = true
@export var interact_name: String = "Ramasser pizza cuite"
var inventory = Inventory 



func _ready():
	add_to_group("pizzacuite")
	visible = true

func interact():
	print("🧺 Pizza ramassée")
	Inventory.AddItem("pizza_cuite")
	queue_free()

	# 🔍 Cherche tous les fours dans la scène
	var ovens = get_tree().get_nodes_in_group("oven")
	for oven in ovens:
		if oven.has_method("check_if_ready_after_space"):
			oven.check_if_ready_after_space()
