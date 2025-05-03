using Godot;
using System;
using System.Collections.Generic;
public partial class EmployeeManager : Node
{

	private Cooker currentCooker;

	public void SpawnCooker()
{
	// Vérifie s’il y a déjà un cuisinier en vie
	if (currentCooker != null && IsInstanceValid(currentCooker))
	{
		GD.PrintErr("❗ Un cuisinier est déjà en poste.");
		return;
	}

	var cookerScene = GD.Load<PackedScene>("res://scenes/employees/Cooker.tscn");
	var cookerInstance = cookerScene.Instantiate() as Cooker;

	if (cookerInstance == null)
	{
		GD.PrintErr("❌ Échec de l’instanciation du cuisinier.");
		return;
	}

	cookerInstance.GlobalPosition = new Vector2(589.405f, 893.0f);
	GetTree().CurrentScene.AddChild(cookerInstance);

	var oven = GetTree().CurrentScene.FindChild("Oven", true, false) as Node2D;
	if (oven != null)
	{
		GD.Print($"🔥 Four trouvé : {oven.Name} à {oven.GlobalPosition}");
		cookerInstance.SetStations(new List<Node2D> { oven });
	}
	else
	{
		GD.PrintErr("❌ Four introuvable !");
	}

	currentCooker = cookerInstance; // ➕ on garde une référence

	GD.Print("👨‍🍳 Nouveau cuisinier spawn à 8h !");
}




}
