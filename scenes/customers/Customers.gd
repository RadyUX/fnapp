extends CharacterBody2D

@onready var sprite = $AnimatedSprite2D

var skin_name = "npc_boy"  

func _ready():
	sprite.play(skin_name)
	
