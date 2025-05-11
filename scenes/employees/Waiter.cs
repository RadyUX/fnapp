using Godot;
using System;

public partial class Waiter : CharacterBody2D
{
	[Export] public float Speed = 80f;
	[Export] public string name = "pepito";
	[Export] public string SkinName = "waiter_1";
	private NavigationAgent2D NavAgent;
	private AnimatedSprite2D Sprite;
	private Node2D currentTarget;
	private Node2D deliveryTarget = null;
	public Inventory inventory;
	private AudioStreamPlayer2D cashSound;

	


	private bool hasPizza = false;
	private int pizzasToCollect = 0;

	private bool isLooping = false;



	public override void _Ready()
	{
		GD.Print("👟 Waiter ready !");
		NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		inventory = new Inventory();
		AddChild(inventory);
		Sprite.Play(SkinName);

		NavAgent.NavigationFinished += OnNavigationFinished;
cashSound = GetNode<AudioStreamPlayer2D>("CashSound");
		StartPizzaLoop();

			var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime += OnClosingTime;
	}
	}

	private void OnNavigationFinished()
	{
		GD.Print("🛑 Arrivé à destination");

		if (currentTarget != null && !hasPizza)
		{
			GD.Print("🍕 Tentative de ramassage via OnNavigationFinished");
			PickUpPizza(currentTarget);
		}
		else if (currentTarget != null && hasPizza)
		{
			GD.Print("📦 Livraison via OnNavigationFinished");
			DeliverToNPC(currentTarget);
		}
	}

	public async void StartPizzaLoop()
	{
		if (isLooping)
			return;

		isLooping = true;

		while (!hasPizza && currentTarget == null)
		{
			GD.Print("🔁 Boucle ➤ relance FindPizza()");
			FindPizza();
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
		}

		isLooping = false;
	}

	public void FindPizza()
{
	

	Node2D closestNpc = null;
	float closestDistance = float.MaxValue;
	int quantity = 0;

	foreach (Node node in GetTree().GetNodesInGroup("npc"))
	{
		if (node is Node2D npc 
			&& IsInstanceValid(npc)
			&& (!npc.HasMeta("in_delivery")
|| !(bool)npc.GetMeta("in_delivery"))
			&& npc.HasMeta("has_pizza") == false
			&& npc.HasMethod("get_request_quantity"))
		{
			var result = npc.Call("get_request_quantity");
			int rq = 0;

			try { rq = (int)(long)result; }
			catch (Exception e) { GD.PrintErr("❌ Erreur de cast get_request_quantity: " + e.Message); }

			if (rq > 0)
			{
				float dist = GlobalPosition.DistanceTo(npc.GlobalPosition);
				if (dist < closestDistance)
				{
					closestDistance = dist;
					closestNpc = npc;
					quantity = rq;
				}
			}
		}
	}

	if (closestNpc == null)
	{
		
		return;
	}

	// ➕ Marquer le NPC comme déjà pris en charge
	closestNpc.SetMeta("in_delivery", true);

	deliveryTarget = closestNpc;
	pizzasToCollect = quantity;
	

	// ➕ Cherche une pizza dispo
	foreach (Node node in GetTree().GetNodesInGroup("pizzacuite"))
	{
		if (node is Area2D pizza && pizza.HasMeta("taken") == false)
		{
			pizza.SetMeta("taken", true);
			currentTarget = pizza;
			NavAgent.TargetPosition = pizza.GlobalPosition;
			return;
		}
	}

	GD.Print("❌ Aucune pizza trouvée pour commencer la commande.");

	// ➖ Reset
	deliveryTarget = null;
	pizzasToCollect = 0;
	currentTarget = null;

	StartPizzaLoop();
}


	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent == null || NavAgent.IsNavigationFinished())
			return;

		Vector2 next = NavAgent.GetNextPathPosition();
		Vector2 dir = (next - GlobalPosition).Normalized();

		Velocity = dir * Speed;
		MoveAndSlide();
		NavAgent.SetVelocity(Velocity);

		if (Mathf.Abs(Velocity.X) > 1f)
			Sprite.FlipH = Velocity.X < 0;

	

		if (GlobalPosition.DistanceTo(NavAgent.TargetPosition) < 10f && currentTarget != null)
		{
			if (!hasPizza)
				PickUpPizza(currentTarget);
			else
				DeliverToNPC(currentTarget);
		}
	}

	private async void PickUpPizza(Node2D pizza)
{
	GD.Print("🤲 Ramassage de pizza...");

	// Appel de l'interaction si disponible
	if (pizza.HasMethod("interact"))
	{
		GD.Print("🎬 Appel de interact() sur pizza");
		pizza.Call("interact");
	}
	else
	{
		GD.Print("⚠️ Pas de méthode interact");
		pizza.QueueFree(); // fallback
	}

	var playerInventory = (Inventory)GetTree().Root.GetNode("Inventory");
	playerInventory.AddItem("pizza_cuite");


	await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

	// Vérifie combien de pizzas on a
	int count = playerInventory.GetItemCount("pizza_cuite");
	GD.Print($"📦 Inventaire actuel : {count} / {pizzasToCollect}");

	if (count < pizzasToCollect)
	{
		GD.Print($"🍕 Il manque encore {pizzasToCollect - count} pizza(s) à ramasser.");
		currentTarget = null;
		StartPizzaLoop();
	}
	else
	{
		hasPizza = true;
		GD.Print("✅ Quantité atteinte ➤ direction NPC");

		if (deliveryTarget == null)
	{
		GD.PrintErr("❌ deliveryTarget est null, impossible de livrer !");
		return;
	}

		currentTarget = deliveryTarget;
		NavAgent.TargetPosition = deliveryTarget.GlobalPosition;
	}
	
}


private void DeliverToNPC(Node2D npc)
{
	

	// Vérifie la commande du NPC
	string requestedItem = npc.Call("get_request_item_name").AsString();
	int quantity = (int)npc.Call("get_request_quantity");

	var playerInventory = (Inventory)GetTree().Root.GetNode("Inventory");
	int available = playerInventory.GetItemCount(requestedItem);
	GD.Print($"📦 Inventaire avant livraison : {available}/{quantity}");

	if (available < quantity)
	{
		GD.Print("❌ Pas assez d’items dans Inventory singleton !");
		hasPizza = false;
		currentTarget = null;
		deliveryTarget = null;
		StartPizzaLoop();
		FindPizza();
		return;
	}

	// Retire les items
	for (int i = 0; i < quantity; i++)
		playerInventory.RemoveItem(requestedItem);

	// NPC reçoit les items
	npc.Call("receive_item", requestedItem, playerInventory);

	// Récompense
	int totalGain = quantity * 3;
	GameStats.Instance.AddProfit(totalGain);

	GD.Print("✅ Livraison acceptée !");
	hasPizza = false;
	currentTarget = null;
	deliveryTarget = null;

	if (cashSound != null)
		cashSound.Play();
	npc.RemoveFromGroup("npc");
	npc.QueueFree();
	StartPizzaLoop();
	FindPizza(); // 💡 Ceci garantit qu’il reparte bosser
}

public void OnClosingTime()
{
	GD.Print("🌙 Fermeture : le serveur rentre chez lui.");

	// Stop toute logique en cours
	currentTarget = null;
	deliveryTarget = null;
	hasPizza = false;
	pizzasToCollect = 0;
	isLooping = false;

	// Se désabonne proprement
	var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime -= OnClosingTime;
	}

	// Optionnel : informer le manager
	var manager = GetTree().CurrentScene.FindChild("EmployeeManager", true, false) as EmployeeManager;

	QueueFree(); // le serveur disparaît
}



public void SetNameTag(string newName)
{
	name = newName;

	var nameLabel = GetNodeOrNull<Label>("NameLabel");
	if (nameLabel != null)
	{
		nameLabel.Text = name;
	}
	else
	{
		GD.PrintErr("⚠️ NameLabel non trouvé !");
	}
}


}
