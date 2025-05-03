extends CanvasLayer


@onready var new_cooker_1: Button = $Panel/TabContainer/Employees/VBoxContainer/NewCooker/newCooker1
@onready var new_mascot: Button = $Panel/TabContainer/Employees/VBoxContainer/NewMascot/newMascot

@export var cooker_1_cost :int = 1000
@export var mascot: int = 10

func _ready():
	new_cooker_1.pressed.connect(_on_new_cooker_1_pressed)
	new_mascot.pressed.connect(_on_new_mascot_1_pressed)


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
