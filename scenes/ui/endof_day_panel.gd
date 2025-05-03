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
	
	
	
	var popularity = GameStats.Popularity
	var income = GameStats.Profit
	var tax_rate = popularity / 10
	var taxes = int(income * tax_rate / 100.0)
	var malus = GameStats.PopularityLoss 
	var net = income - taxes - malus
	net = max(net, 0)

	GameStats.Money -= taxes  
	GameStats.Money += net    

	
		
	popularity_label.text = "Popularité : %d" % popularity
	income_label.text = "Revenu brut : $%d" % income
	taxes_label.text = "Taxes (%d%%) : -$%d" % [tax_rate, taxes]
	malus_label.text = "Malus : -$%d" % malus
	net_label.text = "Revenu net : $%d" % net
	

	show()
