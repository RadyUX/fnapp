class_name DayAndNightCycleComponent
extends CanvasModulate

@export var initial_day: int = 1:
	set(id):
		initial_day = id
		DayAndNightCycleManager.InitialDay = id
		DayAndNightCycleManager.setInitialTime()
	
@export var initial_hour: int = 12:
	set(ih):
		initial_hour = ih
		DayAndNightCycleManager.InitialHour = ih
		DayAndNightCycleManager.setInitialTime()

@export var initial_minute: int = 30:
	set(im):
		initial_minute = im
		DayAndNightCycleManager.InitialMinute = im
		DayAndNightCycleManager.setInitialTime()

@export var day_night_gradient_texture: GradientTexture1D

func _ready() -> void:
		DayAndNightCycleManager.InitialDay = initial_day
		DayAndNightCycleManager.InitialHour = initial_hour
		DayAndNightCycleManager.InitialMinute = initial_minute
		DayAndNightCycleManager.setInitialTime()
		
		DayAndNightCycleManager.GameTime.connect(on_game_time)


func on_game_time(time: float) -> void:
	var sample_value = 0.5 * (sin(time - PI * 0.5) + 1.0)
	color = day_night_gradient_texture.gradient.sample(sample_value)
