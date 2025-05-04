extends CanvasLayer


@onready var new_cooker_1: Button = $Panel/TabContainer/Employees/VBoxContainer/NewCooker/newCooker1
@onready var new_mascot: Button = $Panel/TabContainer/Employees/VBoxContainer/NewMascot/newMascot
@onready var new_waiter_1: Button = $Panel/TabContainer/Employees/VBoxContainer/NewWaiter/newWaiter1

@export var cooker_1_cost :int = 1000
@export var mascot: int = 10
@export var waiter: int = 500

func _ready():
	new_cooker_1.pressed.connect(_on_new_cooker_1_pressed)
	new_mascot.pressed.connect(_on_new_mascot_1_pressed)
	new_waiter_1.pressed.connect(_on_new_waiter_1_pressed)


func _on_new_cooker_1_pressed():
	var manager = EmployeeManager

	if GameStats.Money >= cooker_1_cost:
		GameStats.Money -= cooker_1_cost
		if manager:
			manager.SpawnCooker()
			new_cooker_1.disabled = true
	else:
		print("❌ Pas assez d'argent pour acheter le cuisinier.")


func _on_new_mascot_1_pressed():
	var manager = EmployeeManager

	if GameStats.Money >= mascot:
		GameStats.Money -= mascot
		if manager:
			manager.SpawnMascot() 
			new_mascot.disabled = true  
	else:
		print("❌ Pas assez d'argent pour acheter le mascot.")

func _on_new_waiter_1_pressed():
	var manager = EmployeeManager
	if GameStats.Money >= waiter:
		GameStats.Money -= waiter
		if manager:
			manager.SpawnWaiter() 
	else:
		print("❌ Pas assez d'argent pour acheter le mascot.")
			
		
