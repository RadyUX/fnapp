using Godot;
using System;
using System.Collections.Generic;



public partial class Inventory : Node

{
	// 📦 Liste des items
	   // ✅ Ceci expose le signal à GDScript
	[Signal]
	public delegate void InventoryUpdatedEventHandler();

	public List<string> Items = new List<string>();

	public override void _Ready()
	{
		GD.Print("🎒 Inventaire prêt !");
	
	
		Items.Add("pizza_cuite");
		Items.Add("pizza_cuite");
			Items.Add("pizza_cuite");
	}

	public void AddItem(string itemName)
	{
		Items.Add(itemName);
		GD.Print($"➕ {itemName} ajouté à l'inventaire !");
		EmitSignal(nameof(InventoryUpdated));
	}



	public void RemoveItem(string itemName)
	{
		if (Items.Contains(itemName))
		{
			Items.Remove(itemName);
			GD.Print($"➖ {itemName} retiré de l'inventaire !");
			EmitSignal(nameof(InventoryUpdated));
		}
		else
		{
			GD.Print($"❌ {itemName} non trouvé dans l'inventaire !");
		}
	}

	public bool HasItem(string itemName)
	{
		return Items.Contains(itemName);
	}

	public int GetItemCount(string itemName)
	{
		int count = 0;
		foreach (var item in Items)
		{
			if (item == itemName)
				count++;
		}
		return count;
	}

public Godot.Collections.Dictionary GetAllItemsGDScript()
{
	var itemCounts = new Godot.Collections.Dictionary();
	foreach (var item in Items)
	{
		if (itemCounts.ContainsKey(item))
			itemCounts[item] = (int)itemCounts[item] + 1;
		else
			itemCounts[item] = 1;
	}
	return itemCounts;
}
}
