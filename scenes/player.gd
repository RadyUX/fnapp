extends CharacterBody2D

enum PlayerState {
	IDLE,
	MOVE,
	SWEEP,
	COOK,
	CARRY,
	REPAIR
}
var state = PlayerState.IDLE
var speed = 160
var last_direction = "right" 

func _ready():
	pass
	
	
func _process(delta):
	
	
	var move_vector: Vector2 = Input.get_vector("move_left", "move_right", "move_up", "move_down")
	
	velocity = move_vector * speed

		
	if velocity.x > 0:
		$AnimatedSprite2D.play("walk_right")
		last_direction = "right"
	elif velocity.x < 0:
		$AnimatedSprite2D.play("walk_left")
		last_direction = "left"
	elif velocity.y != 0:
		# Haut et bas utilisent les mêmes anims droite/gauche, à adapter si besoin
		$AnimatedSprite2D.play("walk_" + last_direction)
	else:
		$AnimatedSprite2D.play("idle_" + last_direction)

	move_and_slide()
