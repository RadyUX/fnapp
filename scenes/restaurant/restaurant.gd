extends Node2D
@onready var open_shop: Button = $Buttons/OpenShop

func clear_all_customers():
	var customers = get_tree().get_nodes_in_group("customer")
	for customer in customers:
		customer.force_leave()
		
func _ready():
	DayAndNightCycleManager.ClosingTime.connect(clear_all_customers)
	$Buttons/OpenShop.pressed.connect(_on_open_shop_pressed)

func _on_open_shop_pressed():
	var shop_panel = $ShopPanelUi
	shop_panel.visible = not shop_panel.visible  # toggle on/off
