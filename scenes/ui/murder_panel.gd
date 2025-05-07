extends CanvasLayer


@onready var screamer_audio = $ScreamerAudio
@onready var label = $Panel/BodyDiscoveredLabel
@onready var body_image = $Panel/Body
@onready var close: Button = $Panel/Close

func _ready():
	hide()
	screamer_audio.stop()
	close.pressed.connect(_on_close) 

func trigger_murder_panel():
	show()
	screamer_audio.play()
	label.text = " Un corps a été retrouvé..."


func _on_close():
	hide()
	screamer_audio.stop()
