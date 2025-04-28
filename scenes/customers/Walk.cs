using Godot;
using System;

public partial class Walk : Node2D
{
	[Export] public CharacterBody2D Character;
	[Export] public NavigationAgent2D NavAgent;
	[Export] public AnimatedSprite2D AnimatedSprite;
	[Export] public float MinSpeed = 50f;
	[Export] public float MaxSpeed = 150f;
	[Export] public Rect2 AllowedArea;
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
	}

	private void OnNavFinished()
	{
		AnimatedSprite?.Stop();
		StartWalking();
	}

	private void StartWalking()
	{
		var map = NavAgent.GetNavigationMap();
		var randPos = NavigationServer2D.MapGetRandomPoint(map, NavAgent.NavigationLayers, false);
		NavAgent.TargetPosition = randPos;
		_speed = (float)GD.RandRange(MinSpeed, MaxSpeed);
		GD.Print($"🚶 Next target: {randPos}, speed: {_speed}");
		AnimatedSprite?.Play();
		AnimatedSprite.FlipH = false;
	}

public override void _PhysicsProcess(double delta)
{
	if (NavAgent.IsNavigationFinished())
		return;

	Vector2 nextPos = NavAgent.GetNextPathPosition();
	Vector2 dir = (nextPos - Character.GlobalPosition).Normalized();

	Character.Velocity = dir * _speed;
	Character.MoveAndSlide();

	// Important pour éviter de rester bloqué quand très proche
	if (Character.GlobalPosition.DistanceTo(NavAgent.TargetPosition) < 5.0f)
	{
		NavAgent.SetTargetPosition(NavAgent.TargetPosition); // rafraîchir
	}

	if (AnimatedSprite != null)
		AnimatedSprite.FlipH = dir.X < 0;

}
}
