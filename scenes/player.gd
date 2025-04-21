extends CharacterBody2D


func _ready():
	pass
	
	
func _process(delta):
	var move_vector: Vector2 = Input.get_vector("move_left", "move_right", "move_up", "move_down")
	velocity = move_vector * 20
	
	move_and_slide()
