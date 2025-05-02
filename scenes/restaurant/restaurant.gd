extends Node2D

func clear_all_customers():
	var customers = get_tree().get_nodes_in_group("customer")
	for customer in customers:
		customer.force_leave()
		
func _ready():
	DayAndNightCycleManager.ClosingTime.connect(clear_all_customers)
