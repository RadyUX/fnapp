extends CharacterBody2D
class_name Customer 
@onready var sprite = $AnimatedSprite2D
@onready var item_box: TextureRect = $Control/TextureRect
@onready var item_img: TextureRect = $Control/TextureRect/item_img
@onready var label: Label = $Control/TextureRect/Label
@onready var inventory = get_node("/root/Inventory")


@onready var interactable: Interactable = $DetectionZone
var skin_name = "npc_boy"  

var interact: Callable = func():
	pass


var request_item: Item
var request_quantity: int
var current_order_status: int

@export var is_interactable : bool   = true
@export var interact_name   : String = "Give Order"

func init_customer(item: Item, quantity: int)-> void:
	await ready
	request_item = item
	request_quantity = quantity
	current_order_status = quantity
	show_order_ui()
	
	
func _ready():
	sprite.play(skin_name)
	interactable.interact = _on_interact
	interactable.is_interactable = true
	interactable.interact_name  = interact_name
	

func show_order_ui()-> void:
	item_img.texture = request_item.sprite	
	label.text = str(request_quantity)

func receive_item(item_name: String, inventory: Inventory):
	if request_item == null:
		print("⚠️ Aucun item demandé encore. NPC non initialisé ?")
		return
	if item_name != request_item.name:
		print("❌ Ce n’est pas ce que je veux.")
		return
	if inventory.GetItemCount(item_name) < request_quantity:
		print("⏳ Tu n’as pas encore tout !")
		return

	print("🍕 Merci pour la commande !")

	# Enlève les items maintenant
	for i in range(request_quantity):
		inventory.RemoveItem(item_name)

	print("🧺 Pizza donnée.")


	$cash_sound.play()
	await $cash_sound.finished
	queue_free()


func _on_interact():
	receive_item("pizza_cuite", inventory)
