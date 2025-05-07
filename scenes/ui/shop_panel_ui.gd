extends CanvasLayer

@onready var shop_panel_ui: CanvasLayer = $"."

@onready var new_cooker_1: Button = $Panel/TabContainer/Employees/VBoxContainer/NewCooker/newCooker1
@onready var new_mascot: Button = $Panel/TabContainer/Employees/VBoxContainer/NewMascot/newMascot
@onready var new_waiter_1: Button = $Panel/TabContainer/Employees/VBoxContainer/NewWaiter/newWaiter1

@export var cooker_1_cost :int = 1000
@export var mascot: int = 10
@export var waiter: int = 500
@export var max_waiter : int = 3
@export var max_cooker: int = 1
@export var max_mascot: int = 1
@export var max_janitor: int = 5


func _ready():
	new_cooker_1.pressed.connect(_on_new_cooker_1_pressed)
	new_mascot.pressed.connect(_on_new_mascot_1_pressed)
	new_waiter_1.pressed.connect(_on_new_waiter_1_pressed)
	DayAndNightCycleManager.ClosingTime.connect(_on_closing_time)
	update_waiter_button()
	update_cooker_button()
	update_mascot_button()



func _on_closing_time():
	shop_panel_ui.hide()
	
func _on_new_cooker_1_pressed():
	var current_cookers = get_tree().get_nodes_in_group("cooker").size()
	if current_cookers >= max_cooker:
		print("❌ Maximum atteint.")
		return
	var manager = EmployeeManager

	if GameStats.Money >= cooker_1_cost:
		GameStats.Money -= cooker_1_cost
		if manager:
			manager.SpawnCooker()
			update_cooker_button()
			
	else:
		print("❌ Pas assez d'argent pour acheter le cuisinier.")


func _on_new_mascot_1_pressed():
	var manager = EmployeeManager

	if GameStats.Money >= mascot:
		GameStats.Money -= mascot
		if manager:
			manager.SpawnMascot() 
			update_mascot_button()
	else:
		print("❌ Pas assez d'argent pour acheter le mascot.")


func update_waiter_button():
	var current_waiters = get_tree().get_nodes_in_group("waiter").size()
	new_waiter_1.disabled = current_waiters >= max_waiter
	
func update_cooker_button():
	var current_cooker = get_tree().get_nodes_in_group("cooker").size()
	new_cooker_1.disabled = current_cooker >= max_cooker
	
func update_mascot_button():
	var current_mascot = get_tree().get_nodes_in_group("mascot").size()
	new_mascot.disabled = current_mascot >= max_mascot

func _on_new_waiter_1_pressed():
	var current_waiters = get_tree().get_nodes_in_group("waiter").size()
	if current_waiters >= max_waiter:
		print("❌ Maximum atteint.")
		return
	var manager = EmployeeManager
	if GameStats.Money >= waiter:
		GameStats.Money -= waiter
		if manager:
			manager.SpawnWaiter() 
			update_waiter_button()
	else:
		print("❌ Pas assez d'argent pour acheter le serveur")
			
		
