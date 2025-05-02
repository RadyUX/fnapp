extends CharacterBody2D

@export var skin_name := "cooker_1"

@onready var sprite: AnimatedSprite2D = $AnimatedSprite2D
@onready var nav_agent: NavigationAgent2D = $NavigationAgent2D

func _ready():
	sprite.play(skin_name)
