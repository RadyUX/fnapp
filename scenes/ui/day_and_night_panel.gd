extends Control

@onready var day_label: Label = $DayPanel/MarginContainer/DayLabel
@onready var time_label: Label = $TimePanel/MarginContainer/TimeLabel


func _ready() -> void:
	DayAndNightCycleManager.TimeTick.connect(on_time_tick)


func on_time_tick(day: int, hour: int, minute: int) -> void:
	day_label.text = "DAY " + str(day)
	time_label.text = "%02d:%02d" % [hour, minute]
