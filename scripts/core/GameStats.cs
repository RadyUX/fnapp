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
	[Export] public int Money = 0;              // 💵 Ton porte-monnaie
	[Export] public int NewVisitors = 0;           // 👥 Visiteurs du jour

	[ExportGroup("Malus")]
	[Export] public int TaxRate = 0;               // 📊 % de taxe (calculé auto)
	[Export] public int MoneyLoss = 0;             // 💸 Dépenses (réparations, taxes, achats...)
	[Export] public int PopularityLoss = 0;        // 🔻 Perte de réputation (drames...)

	// 🔻 Total des malus (argent + réputation)
	public int Malus => MoneyLoss + PopularityLoss;


	public int DailyIncome = 0;
public int DailyEntertainmentRevenue = 0;
public int DailyVisitorRevenue = 0;



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
		// Exemple : 1€ par point de divertissement + 5€ par visiteur
		return (Entertainment * 1) + (NewVisitors * 5);
	}

	// 📤 Ajoute un revenu direct (argent et bonus brut)
	public void AddProfit(int amount)
	{

		GD.Print($"[💰 AddProfit] +{amount}$ ajoutés !");
		Money += amount;
		Profit += amount;
		
	}

	public void FinalizeDay()
{
	DailyEntertainmentRevenue = Entertainment * 1;
	DailyVisitorRevenue = NewVisitors * 5;

	int earned = DailyEntertainmentRevenue + DailyVisitorRevenue;

	// ✅ Inclure aussi les gains ajoutés via AddProfit
	earned += Profit; // ou stocker séparément les gains serveurs

	Profit = earned - MoneyLoss;
	if (Profit < 0)
		Profit = 0;

	AddProfit(Profit); // le rajoute à Money

	GD.Print($"💰 Fin du jour : +{DailyEntertainmentRevenue} (fun) +{DailyVisitorRevenue} (clients) +{Profit} (serveur) -{MoneyLoss} = {Profit}€ net");

	UpdatePopularity();
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

	
	public void ApplyMurderPenalty()
{
	int loss = 100_000;
	PopularityLoss += loss;

	GD.Print("🩸 Meurtre détecté ! Popularité -100 000");
	UpdatePopularity();
}

public void DecreaseSafetyDaily(int amount = 5)
{
	Safety -= amount;

	if (Safety < 0)
		Safety = 0;

	GD.Print($"🔐 Sécurité baissée de {amount} ➔ Niveau actuel : {Safety}");

	
}


public void CheckMurderRisk()
{
	int risk = 100 - Safety; // Plus c’est bas, plus c’est risqué
	int roll = GD.RandRange(0, 99); // ou GD.Randi() % 100

	GD.Print($"🎲 Risque de meurtre : {risk}% | Jet : {roll}");

	if (roll < risk)
	{
		GD.Print("💀 MEURTRE ! La sécurité était trop basse.");
		ApplyMurderPenalty();

	
	}
}



public Godot.Collections.Array GetEndOfDaySummary()
{
	int income = Profit;
	int taxRate = Popularity / 10;
	int taxes = (int)(income * (taxRate / 100.0));
	int malus = PopularityLoss;
	int net = income - taxes - malus;
	net = Math.Max(net, 0);

	return new Godot.Collections.Array { taxRate, taxes, malus, net };
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
	
	
	

}
