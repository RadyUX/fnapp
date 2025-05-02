extends Area2D

@export var is_ready_for_cuisson: bool = true
@export var is_interactable: bool = true
@export var interact_name: String = "Pizza crue"
@export var item_data: Item

var interact: Callable = func(): pass

func _ready():
	add_to_group("pizzacru")
