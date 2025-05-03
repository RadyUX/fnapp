using Godot;
using System;
using System.Collections.Generic;
public partial class EmployeeManager : Node
{

	private Cooker currentCooker;
	private Mascot currentMascot;

	private int employeeCount = 0;

	public void RegisterEmployee()
	{
		employeeCount++;
		UpdateSecurity();

		
	}

	private void UpdateSecurity()
{
	// Exemple : base sécurité = 100%, -5% par employé au-delà du premier
	int lossPerEmployee = 5;
	int loss = Math.Max(0, (employeeCount - 1) * lossPerEmployee);
	GameStats.Instance.Safety = Math.Max(0, 100 - loss);

	GD.Print($"🛡️ Sécurité mise à jour : {GameStats.Instance.Safety}%");
}

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
	RegisterEmployee(); 
}


public void SpawnMascot()
 {
		if (currentMascot != null && IsInstanceValid(currentMascot))
		{
			GD.PrintErr("❗ Un mascotte est déjà présente.");
			return;
		}
var mascotScene = GD.Load<PackedScene>("res://scenes/employees/Mascot.tscn");
		var mascotInstance = mascotScene.Instantiate() as Mascot;

		if (mascotInstance == null)
		{
			GD.PrintErr("❌ Échec de l’instanciation du mascot.");
			return;
		}

		mascotInstance.GlobalPosition = new Vector2(199f, 311f); 
		GetTree().CurrentScene.AddChild(mascotInstance);

		currentMascot = mascotInstance;
		GD.Print("🤡 Mascot spawné !");
		RegisterEmployee(); 
	}
 }
