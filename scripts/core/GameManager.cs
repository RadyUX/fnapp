using Godot;
using System;
using System.Collections.Generic; 
public partial class GameManager : Node
{
	private static readonly Resource ITEM_PIZZACUITE = GD.Load<Resource>("res://data/PizzaCuite.tres");
	
	private static readonly Random _rng = new Random();
	public int DaysPassed = 0;
	


	public override void _Ready()
{
	var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");

	if (cycle != null)
	{
		cycle.TimeTickDay += OnNewDay;
	}
}

	public Resource getRandomItem()
	{
		  
		List<Resource> items = new List<Resource> { ITEM_PIZZACUITE };
		int index = _rng.Next(items.Count);
		return items[index];
	
	}


	public void AdvanceDay()
{
	DaysPassed++;
	GameStats.Instance.DecreaseSafety();
	GameStats.Instance.ResetDay();
	
if (DaysPassed == 2)
	{
		GameStats.Instance.CheckMurderRisk();
		EmployeeManager.Instance.ChoosePossessedEmployee();
		GD.Print("👻 Nouvelle semaine ➤ Possession choisie.");
		
	}
	if (DaysPassed % 7 == 0)
	{
		GD.Print("🧨 [DEBUG] CheckMurderRisk() APPELÉ (semaine)");
		GameStats.Instance.CheckMurderRisk();
	}

	// ✅ Puis EndOfDay
	GameStats.Instance.EndOfDay();



	// 🎬 Affiche le résumé sauf au jour 1
	var panel = GetTree().CurrentScene.FindChild("EndOfDayPanel", true, false);
	if (panel == null)
	{
		GD.PrintErr("❌ EndOfDayPanel introuvable !");
	}
	else
	{
		if (DaysPassed == 1)
		{
			GD.Print("⏭️ Jour 1, on saute l’affichage du panel");
			return;
		}

		GD.Print("✅ EndOfDayPanel trouvé !");
		panel.Call("show_summary");
	}
	


if (DaysPassed % 7 == 1) // Le jour après un dimanche, début de semaine
{

		EmployeeManager.Instance.ChoosePossessedEmployee();
		GD.Print("👻 Nouvelle semaine ➤ Possession choisie.");
	
}
}

private void OnNewDay(int day)
{
	GD.Print($"📅 NOUVEAU JOUR {day} ➤ Appel de AdvanceDay()");
	AdvanceDay();
}

}
