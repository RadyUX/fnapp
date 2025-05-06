extends CanvasLayer
@onready var open_shop: Button = $OpenShop



func _ready():
	DayAndNightCycleManager.ClosingTime.connect(_on_closing_time)
	DayAndNightCycleManager.OpeningTime.connect(_on_opening_time)

func _on_closing_time():
	open_shop.hide()


func _on_opening_time():
	open_shop.show()
