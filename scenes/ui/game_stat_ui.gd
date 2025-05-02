extends CanvasLayer

@onready var label_money: Label = $Panel/VBoxContainer/LabelMoney
@onready var label_entertainement: Label = $Panel/VBoxContainer/LabelEntertainement
@onready var label_safety: Label = $Panel/VBoxContainer/LabelSafety
@onready var label_popularity: Label = $Panel/VBoxContainer/LabelPopularity

func _process(delta):
	label_popularity.text = "⭐ Popularité : " + str(GameStats.popularity)
	label_money.text = "💵 Argent : " + str(GameStats.money)
	label_safety.text = "🔐 Sécurité : " + str(GameStats.safety) + "%"
	label_entertainement.text = "🎉 Divertissement : " + str(GameStats.entertainment)
