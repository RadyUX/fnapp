using Godot;
using System;

public partial class DayAndNightCycleManager : Node
{
	// 🔁 Constantes
	public const int MINUTES_PER_DAY = 24 * 60;
	public const int MINUTES_PER_HOUR = 60;
	public readonly float GAME_MINUTE_DURATION;

	// ⚙️ Paramètres de jeu
	public float GameSpeed = 5.0f;

	// 🕒 Données initiales
	public int InitialDay = 1;
	public int InitialHour = 12;
	public int InitialMinute = 30;

	// 🧠 Temps courant
	public float Time = 0.0f;
	public int CurrentMinute = -1;
	public int CurrentDay = 0;

	// 📢 Signaux
	[Signal]
	public delegate void GameTimeEventHandler(float time);

	[Signal]
	public delegate void TimeTickEventHandler(int day, int hour, int minute);

	[Signal]
	public delegate void TimeTickDayEventHandler(int day);

	[Signal]
public delegate void ClosingTimeEventHandler();


	// 🔧 Constructeur
	public DayAndNightCycleManager()
	{
		GAME_MINUTE_DURATION = MathF.Tau / MINUTES_PER_DAY;
	}


   public override void _Ready()
{
	setInitialTime();
}

public override void _Process(double delta)
{
	Time += (float)(delta * GameSpeed * GAME_MINUTE_DURATION);
	EmitSignal(nameof(GameTime), Time);
	RecalculateTime();
}

public void setInitialTime()
{
	var initialTotalMinutes = (InitialDay * MINUTES_PER_DAY) + (InitialHour * MINUTES_PER_HOUR) + InitialMinute;
	Time = initialTotalMinutes * GAME_MINUTE_DURATION;
	GD.Print($"🕒 Temps initial : {Time}");
}


	public void RecalculateTime()
{
	
	int totalMinutes = (int)(Time / GAME_MINUTE_DURATION);

	int day = totalMinutes / MINUTES_PER_DAY;
	int currentDayMinutes = totalMinutes % MINUTES_PER_DAY;

	int hour = currentDayMinutes / MINUTES_PER_HOUR;
	int minute = currentDayMinutes % MINUTES_PER_HOUR;

	// Tick toutes les minutes
	if (minute != CurrentMinute)
	{
		CurrentMinute = minute;
		EmitSignal(nameof(TimeTick), day, hour, minute);
	}

	// Tick à chaque nouveau jour
	if (day != CurrentDay)
	{
		CurrentDay = day;
		EmitSignal(nameof(TimeTickDay), day);
	}
	if (hour == 22 && minute == 0)
	{
		GD.Print("🌙 Il est 22h — on ferme !");

		var panel = GetTree().CurrentScene.FindChild("EndOfDayPanel", true, false);

if (panel == null)
{
	GD.PrintErr("❌ EndOfDayPanel introuvable !");
}
else
{
	GD.Print("✅ EndOfDayPanel trouvé !");
	panel.Call("show_summary");
}


		EmitSignal(nameof(ClosingTime));
	}
if (hour == 8 && minute == 0)
{
	GD.Print("☀️ Il est 8h du mat !");
	var manager = GetNodeOrNull("/root/EmployeeManager"); // ou ton singleton réel
	manager?.Call("SpawnCooker");
}

}
}
