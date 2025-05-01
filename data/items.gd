extends Resource

class_name Item


enum ItemType{
	PizzaCuite,
	Fries,
	Icecream
}
@export var id: String
@export var name: String
@export var type: ItemType
@export var sprite: Texture2D


@export_group("Cook")
@export var cook_time: float = 10.0
@export var cook_time_reduce_perc: float = 0.50

@export_group("Upgrade")
@export var upgrade_cost: float = 3.0
@export var upgrade_multi: float = 1.3

@export_group("Profit")
@export var profit: float = 4.0
@export var profit_multi: float = 1.2

@export_category("Levels")
@export var mex_level: int = 75

var current_level: int  = 0
