using Godot;
using System;
using System.Collections.Generic; 
public partial class GameManager : Node
{
	private static readonly Resource ITEM_PIZZACUITE = GD.Load<Resource>("res://data/PizzaCuite.tres");
	
	private static readonly Random _rng = new Random();
	
	public Resource getRandomItem()
	{
		  
		List<Resource> items = new List<Resource> { ITEM_PIZZACUITE };
		int index = _rng.Next(items.Count);
		return items[index];
	
	}
}
