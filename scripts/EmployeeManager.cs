using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

using System.Runtime.CompilerServices;
public partial class EmployeeManager : Node
{

	public static EmployeeManager Instance { get; private set; }

	private Cooker currentCooker;
	private Mascot currentMascot;

	private Waiter currentWaiter;
	private int maxWaiters = 3;
	private List<Waiter> activeWaiters = new List<Waiter>();

	public List<HiredEmployee> HiredEmployees = new();
	

	private int employeeCount = 0;
	private List<string> availableNames = new()
	{
		"Pepito", "Luigi", "Frederic", "Benoit", "William", "Henry", "Fitz"
	};

	private List<string> usedNames = new();

	public class HiredEmployee
	{
		public string Role;
		public string Name;
		public Vector2 SpawnPosition;

		
	}

	public void RegisterEmployee()
	{
		employeeCount++;
		UpdateSecurity();

		
	}


	public override void _Ready()
	{
		Instance = this;
		for (int i = 0; i < 3; i++)
	{
		SpawnWaiter();
	}
	SpawnCooker();
	SpawnMascot();

	}
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetHiredEmployees()
{
	var result = new Godot.Collections.Array<Godot.Collections.Dictionary>();
	foreach (var emp in HiredEmployees)
	{
		var dict = new Godot.Collections.Dictionary
		{
			{ "name", emp.Name },
			{ "role", emp.Role },
			{ "position", emp.SpawnPosition }
		};
		result.Add(dict);
	}
	return result;
}

	private void UpdateSecurity()
{
	// Exemple : base sécurité = 100%, -5% par employé au-delà du premier
	int lossPerEmployee = 5;
	int loss = Math.Max(0, (employeeCount - 1) * lossPerEmployee);
	GameStats.Instance.Safety = Math.Max(0, 100 - loss);

	GD.Print($"🛡️ Sécurité mise à jour : {GameStats.Instance.Safety}%");
}

	public void SpawnCooker()
{
	// Vérifie s’il y a déjà un cuisinier en vie
	if (currentCooker != null && IsInstanceValid(currentCooker))
	{
		GD.PrintErr("❗ Un cuisinier est déjà en poste.");
		return;
	}

	var cookerScene = GD.Load<PackedScene>("res://scenes/employees/Cooker.tscn");
	var cookerInstance = cookerScene.Instantiate() as Cooker;

	if (cookerInstance == null)
	{
		GD.PrintErr("❌ Échec de l’instanciation du cuisinier.");
		return;
	}

	cookerInstance.GlobalPosition = new Vector2(589.405f, 893.0f);
	GetTree().CurrentScene.AddChild(cookerInstance);

	var oven = GetTree().CurrentScene.FindChild("Oven", true, false) as Node2D;
	if (oven != null)
	{
		GD.Print($"🔥 Four trouvé : {oven.Name} à {oven.GlobalPosition}");
		cookerInstance.SetStations(new List<Node2D> { oven });
	}
	else
	{
		GD.PrintErr("❌ Four introuvable !");
	}

	currentCooker = cookerInstance; // ➕ on garde une référence

	GD.Print("👨‍🍳 Nouveau cuisinier spawn à 8h !");
	RegisterEmployee(); 
	
	string name = AssignEmployeeName();
	cookerInstance.name = name;
	cookerInstance.SetNameTag(name);
	cookerInstance.AddToGroup("cooker");
	cookerInstance.SetMeta("real_name", name);
	HiredEmployees.Add(new HiredEmployee {
	Role = "Cooker",
	Name = name,
	SpawnPosition = cookerInstance.GlobalPosition,
	
});

}


public void SpawnMascot()
 {
		if (currentMascot != null && IsInstanceValid(currentMascot))
		{
			GD.PrintErr("❗ Un mascotte est déjà présente.");
			return;
		}
var mascotScene = GD.Load<PackedScene>("res://scenes/employees/Mascot.tscn");
		var mascotInstance = mascotScene.Instantiate() as Mascot;

		if (mascotInstance == null)
		{
			GD.PrintErr("❌ Échec de l’instanciation du mascot.");
			return;
		}

		mascotInstance.GlobalPosition = new Vector2(199f, 311f); 
		GetTree().CurrentScene.AddChild(mascotInstance);

		currentMascot = mascotInstance;
		GD.Print("🤡 Mascot spawné !");
		RegisterEmployee(); 

	string name = AssignEmployeeName();
	mascotInstance.name = name;
	mascotInstance.SetNameTag(name);
	mascotInstance.AddToGroup("mascot");
	mascotInstance.SetMeta("real_name", name);

	HiredEmployees.Add(new HiredEmployee {
	Role = "Mascot",
	Name = name,
	SpawnPosition = mascotInstance.GlobalPosition
});
	}

	

public void SpawnWaiter()
{
	if (activeWaiters.Count >= maxWaiters)
	{
		GD.PrintErr("🚫 Limite de serveurs atteinte (" + maxWaiters + ")");
		return;
	}

	var WaiterScene = GD.Load<PackedScene>("res://scenes/employees/Waiter.tscn");
	if (WaiterScene == null)
	{
		GD.PrintErr("🚨 WaiterScene n’est pas assigné !");
		return;
	}

	var waiterInstance = WaiterScene.Instantiate() as Waiter;
	if (waiterInstance == null)
	{
		GD.PrintErr("❌ Échec de l’instanciation du serveur.");
		return;
	}

	waiterInstance.GlobalPosition = new Vector2(630.0f, 865.875f); 
	GetTree().CurrentScene.AddChild(waiterInstance);

	activeWaiters.Add(waiterInstance);

	// Callback pour suppression automatique s’il part
	waiterInstance.TreeExited += () =>
	{
		activeWaiters.Remove(waiterInstance);
		GD.Print("👋 Serveur supprimé. Actifs : " + activeWaiters.Count);
	};

	GD.Print("🧍‍♂️ Serveur spawné ! (" + activeWaiters.Count + "/" + maxWaiters + ")");
	RegisterEmployee(); 

	string name = AssignEmployeeName();
	waiterInstance.name = name;
	waiterInstance.SetNameTag(name);
	waiterInstance.AddToGroup("waiter");
	waiterInstance.SetMeta("real_name", name);
	HiredEmployees.Add(new HiredEmployee {
	Role = "Waiter",
	Name = name,
	SpawnPosition = waiterInstance.GlobalPosition
});
}


public string AssignEmployeeName(){
	if(availableNames.Count == 0){
		return "Anonyme";
	}

int index = (int)(GD.Randi() % (ulong)availableNames.Count);

	string chosenName = availableNames[index];
	
	availableNames.RemoveAt(index);
		usedNames.Add(chosenName);

	return chosenName;

}
public void RespawnAllEmployees()
{
	// 🔁 Supprimer les anciens employés
	foreach (Node node in GetTree().CurrentScene.GetChildren())
	{
		if (node is Cooker || node is Mascot || node is Waiter)
			node.QueueFree();
	}

	// 🔁 Respawn depuis HiredEmployees
	foreach (var emp in HiredEmployees)
	{
		string path = $"res://scenes/employees/{emp.Role}.tscn";
		var scene = GD.Load<PackedScene>(path);

		if (emp.Role == "Cooker")
		{
			var cooker = scene.Instantiate() as Cooker;
			if (cooker == null) continue;

			cooker.GlobalPosition = emp.SpawnPosition;
			cooker.Name = emp.Name;
			cooker.SetNameTag(emp.Name);
			cooker.AddToGroup("waiter");
			cooker.SetMeta("real_name", emp.Name);
			GetTree().CurrentScene.AddChild(cooker);

			var oven = GetTree().CurrentScene.FindChild("Oven", true, false) as Node2D;
			if (oven != null)
				cooker.SetStations(new List<Node2D> { oven });

			continue;
		}

		if (emp.Role == "Waiter")
		{
			var waiter = scene.Instantiate() as Waiter;
			if (waiter == null) continue;

			waiter.GlobalPosition = emp.SpawnPosition;
			waiter.Name = emp.Name;
			waiter.SetNameTag(emp.Name);
			waiter.AddToGroup("waiter");
			waiter.SetMeta("real_name", emp.Name);
			GetTree().CurrentScene.AddChild(waiter);
			continue;
		}

		if (emp.Role == "Mascot")
		{
			var mascot = scene.Instantiate() as Mascot;
			if (mascot == null) continue;

			mascot.GlobalPosition = emp.SpawnPosition;
			mascot.Name = emp.Name;
			mascot.SetNameTag(emp.Name);
			mascot.AddToGroup("waiter");
			mascot.SetMeta("real_name", emp.Name);
			GetTree().CurrentScene.AddChild(mascot);
			continue;
		}
	}
}

public void RemoveAllEmployees()
{
	foreach (Node node in GetTree().CurrentScene.GetChildren())
	{
		if (node is Cooker || node is Mascot || node is Waiter)
		{
			node.QueueFree();
			GD.Print($"👋 {node.Name} est rentré chez lui.");
		}
	}
}

public bool firedToday = false;
public void FireEmployee(Dictionary empData){
	   if (firedToday)
	{
		GD.Print("❌ Déjà licencié un employé cette semaine");
		return;
	}

	firedToday = true;
	string empName = (string)empData["name"];
Node employeeNode = null;
string[] groups = { "waiter", "cooker", "mascot" };
GD.Print("🔍 Recherche de : " + empName);

foreach (string group in groups)
{
	foreach (var node in GetTree().GetNodesInGroup(group))
	{
		if (node is Node n && n.HasMeta("real_name"))
		{
			string realName = (string)n.GetMeta("real_name");
			GD.Print($"👁️ Comparaison : {realName} vs {empName}");

			if (realName.Trim().ToLower() == empName.Trim().ToLower())
			{
				employeeNode = n;
				break;
			}
		}
	}
	if (employeeNode != null)
		break;
}

	if (employeeNode != null)
	{
		employeeNode.QueueFree();
		GD.Print($"👋 Employé viré : {empName}");
	}
	else
	{
		GD.PrintErr($"❌ Aucun employé trouvé avec le nom {empName}");
	}

	for (int i = 0; i < HiredEmployees.Count; i++)
		{
			if (HiredEmployees[i].Name == empName)
			{
				HiredEmployees.RemoveAt(i);
				break;
			}
	}
	if (empName == possessedEmployeeName)
{
	GD.Print("👻 L'hôte du fantôme a été viré ! Il va chercher un nouveau corps...");
	possessedEmployeeName = ""; // Vide temporairement

	// Repossède quelqu’un d’autre dans la semaine
	CallDeferred(nameof(ChoosePossessedEmployee));
}
else
{
	GameStats.Instance.ApplyPopularityLoss(-10);
	GD.Print("❌ Mauvais licenciement. Popularité -10.");
}

}


public void ResetFireFlag()
{
	firedToday = false;
	GD.Print("🔄 Reset de firedToday !");
}



private string possessedEmployeeName = "";


/// À appeler au début de chaque semaine
public void ChoosePossessedEmployee()
{
	if (HiredEmployees.Count == 0)
	{
		GD.PrintErr("❌ Aucun employé à posséder !");
		return;
	}

	int index = (int)(GD.Randi() % (ulong)HiredEmployees.Count);
	possessedEmployeeName = HiredEmployees[index].Name;
	GD.Print($"👻 Possession fantomatique : {possessedEmployeeName}");
}

}
