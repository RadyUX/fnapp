extends CanvasLayer

@onready var popularity_label: Label = $Panel/VBoxContainer/PopularityLabel
@onready var income_label: Label = $Panel/VBoxContainer/IncomeLabel
@onready var taxes_label: Label = $Panel/VBoxContainer/TaxesLabel
@onready var malus_label: Label = $Panel/VBoxContainer/MalusLabel
@onready var net_label: Label = $Panel/VBoxContainer/NetLabel
@onready var close_button: Button = $Panel/CloseButton


func _ready():
	close_button.pressed.connect(hide)
	hide()

func show_summary():
	print("📊 show_summary appelé")
	GameStats.FinalizeDay()

	var popularity = GameStats.Popularity
	var income = GameStats.Profit
	var summary = GameStats.GetEndOfDaySummary()
	var tax_rate = summary[0]
	var taxes = summary[1]
	var malus = summary[2]
	var net = summary[3]

	# On applique les effets uniquement maintenant
	GameStats.Money -= taxes
	GameStats.Money += net

	popularity_label.text = "Popularité : %d" % popularity
	income_label.text = "Revenu brut : $%d" % income
	taxes_label.text = "Taxes (%d%%) : -$%d" % [tax_rate, taxes]
	malus_label.text = "Malus : -$%d" % malus
	net_label.text = "Revenu net : $%d" % net # pas GameStats.Money ici !


	show()
