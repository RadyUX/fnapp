extends CanvasLayer
@onready var open_shop: Button = $OpenShop
@onready var open_fire_panel: Button = $OpenFirePanel
@onready var fire_employee_panel: CanvasLayer = $"../FireEmployeePanel"


func _ready():
	DayAndNightCycleManager.ClosingTime.connect(_on_closing_time)
	DayAndNightCycleManager.OpeningTime.connect(_on_opening_time)
	open_fire_panel.pressed.connect(_on_open_fire_panel_pressed)

func _on_closing_time():
	open_shop.hide()


func _on_opening_time():
	open_shop.show()
	open_fire_panel.show()


func _on_open_fire_panel_pressed():
	if fire_employee_panel.visible:
		print("❌ Fermeture du panneau de licenciement")
		fire_employee_panel.hide()
	else:
		print("🧨 Ouverture du panneau de licenciement")
		fire_employee_panel.show_fire_panel()
