extends CanvasLayer

@onready var employee_list: VBoxContainer = $Panel/EmployeeList




func _ready():
	hide()

	
	
func show_fire_panel():
	var manager = get_node("/root/EmployeeManager")

	# 🧼 Vider la liste actuelle
	for child in employee_list.get_children():
		child.queue_free()

	# 🧩 Recréer les éléments
	for emp_data in manager.GetHiredEmployees():
		var row = HBoxContainer.new()

	
		# 🏷️ Nom de l'employé
		var label = Label.new()
		label.text = emp_data["name"]
		label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		label.add_theme_color_override("font_color", Color.WHITE)
		label.custom_minimum_size = Vector2(150, 30)
		row.add_child(label)

		employee_list.add_child(row)
			# 👢 Bouton "Virer" en rouge
		var fire_button = Button.new()
		fire_button.text = "Virer"
		fire_button.add_theme_color_override("button_color", Color.RED)
		fire_button.pressed.connect(
			func():
				print("👋 " + emp_data["name"] + " a été viré.")
				manager.FireEmployee(emp_data) # à implémenter dans EmployeeManager
				show_fire_panel() # refresh
		)
		row.add_child(fire_button)
		


	show()
