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



	

}

private void OnNewDay(int day)
{
	GD.Print($"📅 NOUVEAU JOUR {day} ➤ Appel de AdvanceDay()");
	AdvanceDay();
}

}
