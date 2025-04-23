extends Area2D

@export var is_interactable: bool = true
@export var interact_name: String = "Ramasser pizza cuite"

var interact: Callable = func():
	pass

func _ready():
	add_to_group("pizzacuite")
	visible = true

	interact = func():
		print("🧺 Pizza ramassée")
		queue_free()

		# 🔍 Cherche tous les fours dans la scène
		var ovens = get_tree().get_nodes_in_group("oven")
		for oven in ovens:
			if oven.has_method("check_if_ready_after_space"):
				print("✅ Appel de check_if_ready_after_space() sur", oven.name)
				oven.check_if_ready_after_space()
