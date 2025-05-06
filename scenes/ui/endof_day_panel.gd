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


	await get_tree().process_frame  # <<< ATTENDS une frame que GameStats finisse son taf

	# Maintenant, on récupère des données MÀJ
	var summary = GameStats.get_summary()

	print("💣 Résumé brut : ", summary)

	income_label.text = "Revenu brut : $%d" % summary["gross"]
	taxes_label.text = "Taxes (%d%%) : -$%d" % [summary["tax_rate"], summary["taxes"]]
	malus_label.text = "Malus : -$%d" % summary["malus"]
	net_label.text = "Revenu net : $%d" % summary["net"]
	popularity_label.text = "Popularité : %d" % summary["popularity"]

	show()
