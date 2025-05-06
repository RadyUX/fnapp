using Godot;
using System;

public partial class GameStats : Node
{
	public static GameStats Instance;

	[Export] public int Popularity { get; set; } = 0;
	[Export] public int Safety { get; set; } = 100;
	[Export] public int Money { get; set; } = 10000;

	[Export] public int Entertainment { get; set; } = 0;
	[Export] public int PopularityLoss { get; set; } = 0;
	[Export] public int TaxRate { get; set; } = 0;

	private int DailyRevenue = 0;     // 💰 Total gagné aujourd'hui
	private int DailyMalus = 0;       // 💀 Pertes de réputation (événements)
	private int DailyGross = 0;       // 🧾 Revenu brut = ce qu'on a généré
	private int DailyNet = 0;         // 🧮 Revenu net = ce qui reste après malus/taxes

	public override void _Ready()
	{
		Instance = this;
		RecalculateStats();
	}

	// 🔁 Calcul des stats
	public void RecalculateStats()
	{
		CalculateTaxRate();
		UpdatePopularity();
	}

	public void CalculateTaxRate()
	{
		TaxRate = Popularity / 10;
		TaxRate = Mathf.Clamp(TaxRate, 0, 30);
	}

	public void UpdatePopularity()
	{
		Popularity = Entertainment + DailyRevenue - PopularityLoss;
		Popularity = Mathf.Max(Popularity, 0);
	}

	// 📥 Ajoute un gain direct (pizza, serveur, etc.)
	public void AddProfit(int amount)
	{
		Money += amount;
		DailyRevenue += amount;
		GD.Print($"💰 Gagné +{amount}€, Wallet : {Money}€");
		UpdatePopularity();
	}

	// 🎊 Ajoute du divertissement
	public void AddEntertainment(int amount)
	{
		Entertainment += amount;
		UpdatePopularity();
	}

	// 📉 Ajoute un malus à la popularité
	public void ApplyPopularityLoss(int amount)
	{
		PopularityLoss += amount;
		UpdatePopularity();
	}

	// 🔐 Baisse de sécurité quotidienne
	public void DecreaseSafety(int amount = 5)
	{
		Safety -= amount;
		if (Safety < 0)
			Safety = 0;
		GD.Print($"🔐 Sécurité -{amount}, actuelle : {Safety}");
	}

	// 📅 Fin de journée
	private bool hasEndedToday = false;

public void EndOfDay()
{
	if (hasEndedToday)
	{
		GD.Print("⏱ EndOfDay() déjà appelé !");
		return;
	}
	hasEndedToday = true;

	// ⏳ FIN DE JOURNÉE ICI
	RecalculateStats();
	CheckMurderRisk();

	DailyGross = DailyRevenue;
	int taxes = GetTaxes();
	DailyMalus = PopularityLoss;
	DailyNet = DailyGross - taxes - DailyMalus;
	DailyNet = Mathf.Max(DailyNet, 0);

	Money -= taxes;
	Money += DailyNet;

int lossMoney = PopularityLoss * 2;
Money -= lossMoney;
DailyNet -= lossMoney; // refléter la vraie perte dans le résumé

GD.Print($"😖 Popularité -{PopularityLoss} ➜ Perte d'argent : -{lossMoney}€");


	GD.Print($"📅 Fin du jour !\n🍕 Revenu : {DailyGross}€\n📉 Malus : {DailyMalus}\n📊 Taxes : -{taxes}€\n💼 Wallet : {Money}€");

	DecreaseSafety();

	RecalculateStats();
}

public void ResetDay()
{
	hasEndedToday = false;
}

	public int GetTaxes()
	{
		return Mathf.FloorToInt(DailyRevenue * (TaxRate / 100f));
	}

	public void CheckMurderRisk()
	{
		int risk = 100 - Safety;
		int roll = GD.RandRange(0, 99);
		GD.Print($"🎲 Risque de meurtre : {risk}% | Jet : {roll}");

		if (roll < risk)
		{
			GD.Print("💀 MEURTRE ! La sécurité était trop basse.");
			ApplyMurderPenalty();
		}
	}

	public void ApplyMurderPenalty()
	{
		int loss = 100_000;
		PopularityLoss += loss;
		GD.Print("🩸 Meurtre détecté ! Popularité -100 000");
		UpdatePopularity();
	}

	// === Exposés à GDScript ===
	public void end_of_day() => EndOfDay();
	public void add_profit(int amount) => AddProfit(amount);
	public void add_entertainment(int amount) => AddEntertainment(amount);
	public void apply_popularity_loss(int amount) => ApplyPopularityLoss(amount);

	public int money
	{
		get => Money;
		set => Money = value;
	}
	public int popularity => Popularity;
	public int safety => Safety;
	public int entertainment => Entertainment;
	public int profit => DailyRevenue;

	public Godot.Collections.Dictionary get_summary() => GetSummary();
	public Godot.Collections.Dictionary GetSummary()
	{
		return new Godot.Collections.Dictionary
		{
			{ "tax_rate", TaxRate },
			{ "taxes", GetTaxes() },
			{ "malus", DailyMalus },
			{ "net", DailyNet },
			{ "gross", DailyGross },
			{ "popularity", Popularity }
		};
	}
}
