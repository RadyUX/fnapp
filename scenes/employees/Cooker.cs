using Godot;
using System;
using System.Collections.Generic;

public partial class Cooking : Node2D
{
	[Export] public CharacterBody2D Character;
	[Export] public NavigationAgent2D NavAgent;
	[Export] public AnimatedSprite2D AnimatedSprite;
	[Export] public NodePath[] WorkStations;


	[Signal]
	public delegate void RequestStateChangeEventHandler(string nextState);

	
	private int currentStationIndex = 0;
	private float speed = 80f;
	private List<Node2D> stationInstances = new();

public override void _Ready()
{
	foreach (var path in WorkStations)
	{
		var station = GetNode<Node2D>(path);
		if (station != null)
			stationInstances.Add(station);
		else
			GD.PrintErr($"❌ Station introuvable à {path}");
	}

	NavAgent.NavigationFinished += OnNavigationFinished;
}

	public void OnEnter()
	{
		GD.Print("🔁 [Cooking] Start");

		// Instancie UNE FOIS
	
		StartCookingCycle();
	}


	private void StartCookingCycle()
	{
		if (stationInstances.Count == 0)
		{
			GD.PrintErr("❌ Aucune station instanciée.");
			return;
		}

		var target = stationInstances[currentStationIndex].GlobalPosition;
		NavAgent.TargetPosition = target;
		GD.Print($"🎯 Cuisinier se déplace vers {target}");

		

		currentStationIndex = (currentStationIndex + 1) % stationInstances.Count;
	}

	private void OnNavigationFinished()
	{
		GD.Print("🍽️ Cuisinier est arrivé à la station.");

		Character.Velocity = Vector2.Zero;
		AnimatedSprite?.Play("idle");

		// Attente avant de repartir
		var timer = GetTree().CreateTimer(3.0);
		timer.Timeout += () => EmitSignal("RequestStateChange", "Cooking");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent.IsNavigationFinished())
			return;

		Vector2 next = NavAgent.GetNextPathPosition();
		Vector2 direction = (next - Character.GlobalPosition).Normalized();

		Character.Velocity = direction * speed;
		Character.MoveAndSlide();

		if (AnimatedSprite != null)
			AnimatedSprite.FlipH = direction.X < 0;
	}
}
