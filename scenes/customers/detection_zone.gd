extends Area2D
class_name Interactable

# ✅ Ce Callable sera défini dynamiquement par le node parent (ex : Customer)
var interact: Callable = func():
	pass

# ✅ Nom qui s’affichera dans le label d’interaction
var interact_name: String = "Interagir"

# ✅ Si false, ce node ne sera pas pris en compte par interact.gd
var is_interactable: bool = true
