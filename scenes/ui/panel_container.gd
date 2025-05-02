extends PanelContainer

@onready var inventory_scene: PackedScene = preload("res://scenes/restaurant/Inventory.tscn")
@onready var items_list = $ItemList
@export var item_slot_scene: PackedScene

var inventory: Node = null

func _ready():
	Inventory.InventoryUpdated.connect(refresh_inventory)
	refresh_inventory()

func refresh_inventory():
	items_list.get_children().map(func(child): child.queue_free())
	
	var all_items = Inventory.call("GetAllItemsGDScript")
	print("✅ Items chargés :", all_items)
	print("🎯 all_items =", all_items)
	for item_name in all_items.keys():
		var quantity = all_items[item_name]
		var item_slot = item_slot_scene.instantiate()
		item_slot.setup(item_name, quantity)
		items_list.add_child(item_slot)
