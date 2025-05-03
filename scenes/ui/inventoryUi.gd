extends VBoxContainer


@onready var icon: TextureRect = $ItemIcon
@onready var name_label: Label = $ItemName
@onready var quantity_label: Label = $ItemQuantity

func setup(item: Item, quantity: int) -> void:
	icon.texture = item.sprite
	name_label.text = item.name
	quantity_label.text = "x" + str(quantity)
