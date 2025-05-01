using Godot;
using System;

public partial class Walk : Node2D
{
	[Export] public CharacterBody2D Character;
	[Export] public NavigationAgent2D NavAgent;
	[Export] public AnimatedSprite2D AnimatedSprite;
	[Export] public float MinSpeed = 50f;
	[Export] public float MaxSpeed = 150f;
	[Export] public Rect2 PreferredZone = new Rect2(0, 0, 1200, 500); // x, y, width, height
	[Export] public Area2D bump;
[Signal]
public delegate void RequestStateChangeEventHandler(string nextState);
	private float _speed;

	public override void _Ready()
	{
		if (Character == null || NavAgent == null)
		{
			GD.PrintErr("🛑 Exports non assignés !");
			return;
		}
		if (NavAgent.GetNavigationMap() == null)
		{
			GD.PrintErr("🛑 Pas de NavigationRegion2D/Polygon !");
			return;
		}

		NavAgent.NavigationFinished += OnNavFinished;
		StartWalking();

		if (bump == null)
		{
			GD.PrintErr("❌ BumpDetector introuvable !");
		}
		else
		{
			GD.Print("✅ BumpDetector détecté avec succès !");
			bump.AreaEntered += OnAreaEntered;
		}

		AddToGroup("npc");
	}

	private void OnAreaEntered(Area2D area)
	{
		if (!area.IsInGroup("npc"))
			return;

	

		if (NavAgent != null)
		{
			NavAgent.TargetPosition = GlobalPosition;
			StartWalking();
		}
	}

	private void OnNavFinished()
{
	
	EmitSignal("RequestStateChange", "Idle");
}

	private void StartWalking()
{
	var map = NavAgent.GetNavigationMap();

	for (int i = 0; i < 15; i++) // On tente 15 positions max
	{
		// 👣 1. Point aléatoire sur la map
		Vector2 randPos = NavigationServer2D.MapGetRandomPoint(map, NavAgent.NavigationLayers, false);

		// 👁 2. Éviter les positions trop proches
		if (Character.GlobalPosition.DistanceTo(randPos) < 50)
		{
			GD.Print("❌ Trop proche, on skip.");
			continue;
		}

		// 🔀 3. Vérifie qu’il y a un chemin faisable
		var path = NavigationServer2D.MapGetPath(map, Character.GlobalPosition, randPos, false);
		if (path.Length <= 1)
		{
			GD.Print("❌ Chemin invalide, on skip.");
			continue;
		}

		// ✅ 4. Tout est bon : on y va
		NavAgent.TargetPosition = randPos;
		_speed = (float)GD.RandRange(MinSpeed, MaxSpeed);
		AnimatedSprite?.Play();
		AnimatedSprite.FlipH = false;
		
		return;
	}

	// 😵 Si on trouve rien après 15 essais, on fait rien
	GD.PrintErr("❌ Aucun point valide trouvé !");
}

	public override void _PhysicsProcess(double delta)
	{
		if (NavAgent.IsNavigationFinished())
			return;

		Vector2 nextPos = NavAgent.GetNextPathPosition();
		Vector2 dir = (nextPos - Character.GlobalPosition).Normalized();

		Character.Velocity = dir * _speed;
		Character.MoveAndSlide();

		if (Character.GlobalPosition.DistanceTo(NavAgent.TargetPosition) < 5.0f)
		{
			NavAgent.SetTargetPosition(NavAgent.TargetPosition);
		}

		if (AnimatedSprite != null)
			AnimatedSprite.FlipH = dir.X < 0;

			
if (Character.GetSlideCollisionCount() > 0)
{
	
	StartWalking();
	return;
}
	}
}
