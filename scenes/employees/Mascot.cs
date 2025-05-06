using Godot;
using System;
public partial class Mascot : CharacterBody2D
{
	[Export] public int entertainmentPerSecond = 1;
	[Export] public float Speed = 50f;

	private float timer = 0f;

	private AnimatedSprite2D sprite;

	private NavigationAgent2D navAgent;
	private Vector2 targetPos;
	private RandomNumberGenerator rng = new();
	public string name = "???";

	public override void _Ready()
	
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	   sprite.Play("default");
	   rng.Randomize();
	   navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

	   navAgent.NavigationFinished += OnNavigationFinished;

	   	ChooseNewTarget();

	var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime += OnClosingTime;
		GD.Print("✅ Mascot connecté au signal de fermeture !");
	}
	else
	{
		GD.PrintErr("❌ Cycle jour/nuit introuvable !");
	}

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


	public override void _Process(double delta)
	{
		timer += (float)delta;

		if (timer >= 1f)
		{
			AddEntertainmentPoints(entertainmentPerSecond);
			timer = 0f;
		}
	}


public override void _PhysicsProcess(double delta)
{
	if (navAgent == null || navAgent.IsNavigationFinished())
		return;

	Vector2 next = navAgent.GetNextPathPosition();
	Vector2 dir = (next - GlobalPosition).Normalized();

	Velocity = dir * Speed;
	MoveAndSlide();

	navAgent.SetVelocity(Velocity);

	// 🎭 Flip du sprite selon la direction
	if (sprite != null && Mathf.Abs(Velocity.X) > 1f)
		sprite.FlipH = Velocity.X < 0;
}

	private void AddEntertainmentPoints(int amount)
	{
		GameStats.Instance.Entertainment += amount;
;

		
	}



	private void ChooseNewTarget()
{
	// Génère une position aléatoire dans la NavigationRegion
	Vector2 randomOffset = new Vector2(rng.RandfRange(-300, 300), rng.RandfRange(-200, 200));
	Vector2 target = GlobalPosition + randomOffset;

	navAgent.TargetPosition = target;
	GD.Print($"🎯 Nouveau déplacement vers {target}");
}

private async void OnNavigationFinished()
{
	GD.Print("🛑 Mascot arrivé à destination.");

	await ToSignal(GetTree().CreateTimer(1.0), "timeout");

	ChooseNewTarget();
}


public void OnClosingTime()
{
	GD.Print("🎭 mascot dude termine son show et rentre chez lui.");

	var cycle = GetNodeOrNull<DayAndNightCycleManager>("/root/DayAndNightCycleManager");
	if (cycle != null)
	{
		cycle.ClosingTime -= OnClosingTime;
	}

	// Libère la référence dans EmployeeManager
	var manager = GetTree().CurrentScene.FindChild("EmployeeManager", true, false) as EmployeeManager;
	
	QueueFree();
}

}
