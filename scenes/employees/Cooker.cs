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

	
	public override void _Ready()
	{
		GD.Print("🧠 _Ready() appelé.");
	
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
		_navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		if (_animatedSprite != null)
		{
			GD.Print("✅ AnimatedSprite2D trouvé !");
			_animatedSprite.Play(SkinName);
		}
		else
		{
			GD.PrintErr("❌ AnimatedSprite2D introuvable !");
		}

		if (_navAgent != null)
		{
			GD.Print("✅ NavigationAgent2D trouvé !");
			_navAgent.NavigationFinished += OnNavigationFinished;
		}
		else
		{
			GD.PrintErr("❌ NavigationAgent2D introuvable !");
		}

		foreach (var path in WorkStations)
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

		StartCookingCycle();
	}

	private void StartCookingCycle()
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
	QueueFree(); // ou une animation de départ si tu préfères
}
}
