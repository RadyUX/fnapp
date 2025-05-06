using Godot;
using System;
using System.Collections.Generic;

public partial class Cooker : CharacterBody2D
{
	[Export] public string SkinName = "cooker_1";
	[Export] public NodePath[] WorkStations;
	[Export] public CharacterBody2D Character;

	private AnimatedSprite2D _animatedSprite;
	private NavigationAgent2D _navAgent;

	private int currentStationIndex = 0;
	private float speed = 80f;
	private List<Node2D> stationInstances = new();
	public string name = "???";

	public override void _Ready()
	{

		
		GD.Print("🧠 _Ready() appelé.");
		Velocity = Vector2.Zero;
_navAgent?.SetVelocity(Vector2.Zero);
		
	GD.Print("👨‍🍳 Cooker prêt !");
	
	var cycle = GetNode<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime += OnClosingTime; // ✅ ici c’est valide
		GD.Print("✅ Connecté au signal de fermeture !");
	
	}
	else
	{
		GD.PrintErr("❌ Cycle jour/nuit introuvable !");
	}

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_navAgent = GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D");
		if (_navAgent != null)
		{
			GD.Print("✅ NavigationAgent2D trouvé !");
			_navAgent.NavigationFinished += OnNavigationFinished;
		}
		else
		{
			GD.PrintErr("❌ NavigationAgent2D introuvable !");
		}


		if (_animatedSprite != null)
		{
			GD.Print("✅ AnimatedSprite2D trouvé !");
			_animatedSprite.Play(SkinName);
		}
		else
		{
			GD.PrintErr("❌ AnimatedSprite2D introuvable !");
		}

		

		foreach (var path in WorkStations?? Array.Empty<NodePath>())
		{
			GD.Print($"🔍 Tentative de chargement station : {path}");
			var station = GetNodeOrNull<Node2D>(path);
			if (station != null)
			{
				stationInstances.Add(station);
				GD.Print($"✅ Station ajoutée : {station.Name}");
			}
			else
			{
				GD.PrintErr($"❌ Station introuvable : {path}");
			}
		}

		
	}

	public void SetStations(List<Node2D> stations)
{
	if (stations == null || stations.Count == 0)
	{
		GD.PrintErr("❌ Liste des stations vide ou null !");
		return;
	}

	stationInstances = stations;

	foreach (var station in stationInstances)
	{
		if (station == null)
		{
			GD.PrintErr("❌ Une station reçue est NULL !");
		}
		else
		{
			GD.Print($"✅ Station assignée : {station.Name} à {station.GlobalPosition}");
		}
	}

	// Réinitialise position et vitesse pour éviter un bug
	Velocity = Vector2.Zero;
	_navAgent?.SetVelocity(Vector2.Zero);

	// Lance le cycle après s'être assuré que tout est prêt
	CallDeferred(nameof(StartCookingCycle));
}

	private async void StartCookingCycle()
{
	GD.Print("🚶‍♂️ StartCookingCycle()");

	if (stationInstances.Count == 0)
	{
		GD.PrintErr("❌ Pas de stations à visiter.");
		return;
	}

	var target = stationInstances[currentStationIndex].GlobalPosition;
	GD.Print($"🎯 Déplacement vers la station #{currentStationIndex} à {target}");

	_navAgent.TargetPosition = target;

	// 🔒 ATTEND que le path soit prêt
	await ToSignal(_navAgent, "path_changed");

	currentStationIndex = (currentStationIndex + 1) % stationInstances.Count;
}


	private void OnNavigationFinished()
	{
		GD.Print("✅ OnNavigationFinished() ➜ Le cuisinier est arrivé.");

		Character.Velocity = Vector2.Zero;
		_animatedSprite?.Play(SkinName);

		var station = stationInstances[(currentStationIndex + stationInstances.Count - 1) % stationInstances.Count];

		if (station != null)
		{
			GD.Print($"🔧 Station actuelle : {station.Name}");

			if (station.HasMethod("launch_cooking"))
			{
				GD.Print("🍕 Appel de launch_cooking() sur le four...");
				station.Call("launch_cooking");
			}
			else
			{
				GD.PrintErr("❌ La station n’a pas de méthode launch_cooking.");
			}
		}
		else
		{
			GD.PrintErr("❌ Station null dans OnNavigationFinished().");
		}

		var timer = GetTree().CreateTimer(3.0);
		timer.Timeout += StartCookingCycle;
	}

	public override void _PhysicsProcess(double delta)
{
	if (_navAgent == null)
		return;

	if (_navAgent.IsNavigationFinished())
		return;

	Vector2 next = _navAgent.GetNextPathPosition();
	Vector2 direction = (next - GlobalPosition).Normalized();

	Velocity = direction * speed;
	MoveAndSlide();

	// 🔥 DIT au NavigationAgent que t’as avancé
	_navAgent.SetVelocity(Velocity);

	if (_animatedSprite != null)
		_animatedSprite.FlipH = direction.X < 0;

		GD.Print($"🧭 Velocity actuelle : {Velocity}");
GD.Print($"📍 Position actuelle : {GlobalPosition}");

}

public void OnClosingTime()
{
	GD.Print("👨‍🍳 Il est 22h, je rentre !");

	var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime -= OnClosingTime;
		cycle.TimeTick -= OnTimeTick;
	}

	QueueFree();
}

public void OnTimeTick(int day, int hour, int minute)
{
	// Tu peux ajouter du code ici si tu veux que le cuisinier revive à 8h
	GD.Print($"🕗 Tick reçu : {hour}h{minute} — (Cooker déjà mort)");
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
