using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Node
{
	// 📦 Liste des items
	public List<string> Items = new List<string>();

	public override void _Ready()
	{
		GD.Print("🎒 Inventaire prêt !");
	}

	public void AddItem(string itemName)
	{
		Items.Add(itemName);
		GD.Print($"➕ {itemName} ajouté à l'inventaire !");
	}

	public void RemoveItem(string itemName)
	{
		if (Items.Contains(itemName))
		{
			Items.Remove(itemName);
			GD.Print($"➖ {itemName} retiré de l'inventaire !");
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
}
