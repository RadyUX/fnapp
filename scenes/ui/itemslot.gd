extends HBoxContainer

@export var item_icon: TextureRect 
@export var item_quantity: Label 
@export var item_name: Label

func setup(item_name_str: String, quantity: int) -> void:
	print("📦 setup appelé avec :", item_name_str, quantity)
	item_name.text = item_name_str
	item_quantity.text = "x" + str(quantity)
