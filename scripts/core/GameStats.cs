using Godot;
using System;

public partial class GameStats : Node
{
	public static GameStats Instance;

	[ExportGroup("Stats")]
	[Export] public int Popularity = 0;           // ⭐ Célèbre ou pas
	[Export] public int Entertainment = 0;         // 🎉 Shows, mascottes...
	[Export] public int Profit = 0;                // 💰 Ce que tu gagnes après pertes
	[Export] public int Safety = 100;              // 🔐 Sécurité du resto
	[Export] public int Money = 1000;              // 💵 Ton porte-monnaie
	[Export] public int NewVisitors = 0;           // 👥 Visiteurs du jour

	[ExportGroup("Malus")]
	[Export] public int TaxRate = 0;               // 📊 % de taxe (calculé auto)
	[Export] public int MoneyLoss = 0;             // 💸 Dépenses (réparations, taxes, achats...)
	[Export] public int PopularityLoss = 0;        // 🔻 Perte de réputation (drames...)

	// 🔻 Total des malus (argent + réputation)
	public int Malus => MoneyLoss + PopularityLoss;


	public override void _Ready()
	{
		Instance = this;
		UpdateProfit(); // au cas où
		UpdatePopularity();
	}

	// 📈 Recalcule la popularité globale
	public void UpdatePopularity()
	{
		Popularity = Entertainment + Profit - PopularityLoss;
		if (Popularity < 0)
			Popularity = 0;
	}

	// 💰 Recalcule le vrai profit du jour
	public void UpdateProfit()
	{
		Profit = GetGrossProfit() - MoneyLoss;
		if (Profit < 0)
			Profit = 0;

		UpdatePopularity();
	}

	// 🧮 Calcule les revenus avant pertes (personnalisable selon ton gameplay)
	public int GetGrossProfit()
	{
		// Exemple : 10€ par point de divertissement + 5€ par visiteur
		return (Entertainment * 10) + (NewVisitors * 5);
	}

	// 📤 Ajoute un revenu direct (argent et bonus brut)
	public void AddProfit(int amount)
	{
		Money += amount;
		UpdateProfit();
	}

	// 💸 Dépense de l’argent (achat, réparation, etc.)
	public void SpendMoney(int amount)
	{
		Money -= amount;
		MoneyLoss += amount;
		UpdateProfit();
	}

	// 🎊 Ajoute du divertissement (par show, animatronique…)
	public void AddEntertainment(int value)
	{
		Entertainment += value;
		UpdateProfit();
	}

	// 📊 Applique la taxe en fin de journée selon la popularité
	public void ApplyTax()
	{
		TaxRate = Popularity / 10; // +1% tous les 10 points
		int taxAmount = (int)(GetGrossProfit() * (TaxRate / 100.0));

		GD.Print($"📊 Popularité : {Popularity} ➔ Taxe : {TaxRate}%");
		GD.Print($"💸 Taxe appliquée : -{taxAmount}€");

		Money -= taxAmount;
		MoneyLoss += taxAmount;

		UpdateProfit();
	}

	// 🔄 Réinitialise les stats journalières à la fin du jour
	public void ResetDayStats()
	{
		Entertainment = 0;
		NewVisitors = 0;
		MoneyLoss = 0;
		PopularityLoss = 0;
		TaxRate = 0;

		UpdateProfit();
	}

	public void ApplyMurderPenalty()
{
	int loss = 100_000;
	PopularityLoss += loss;

	GD.Print("🩸 Meurtre détecté ! Popularité -100 000");
	UpdatePopularity();
}


	// === Compatibilité GDScript ===
	public int popularity => Popularity;
	public int money => Money;
	public int safety => Safety;
	public int entertainment => Entertainment;

		// === Méthodes accessibles depuis GDScript ===
	public void add_profit(int amount) => AddProfit(amount);
	public void spend_money(int amount) => SpendMoney(amount);
	public void add_entertainment(int value) => AddEntertainment(value);
	public void apply_tax() => ApplyTax();
	public void reset_day_stats() => ResetDayStats();
	

}
