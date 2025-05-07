using Godot;
using System;

public partial class Idle : Node
{
	[Export] public float MinWait = 1.5f;
	[Export] public float MaxWait = 3.5f;

	[Signal]
	public delegate void RequestStateChangeEventHandler(string nextState);

	private Timer waitTimer;

	public override void _Ready()
	{
		waitTimer = new Timer();
		waitTimer.OneShot = true;
		AddChild(waitTimer);
		waitTimer.Timeout += OnWaitFinished;
	}

[Export] public AnimatedSprite2D AnimatedSprite;
[Export] public CharacterBody2D Character;

public void OnEnter()
{
	float waitTime = (float)GD.RandRange(MinWait, MaxWait);

	// 🛑 Stop animation + mouvement
	if (AnimatedSprite != null)
		AnimatedSprite.Stop();

	if (Character != null)
		Character.Velocity = Vector2.Zero;

	waitTimer.Start(waitTime);

}
	public void OnExit()
	{
		waitTimer.Stop();
	}

	private void OnWaitFinished()
	{
		
		EmitSignal("RequestStateChange", "Walk");
	}
}
