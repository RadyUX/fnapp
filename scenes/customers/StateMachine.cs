using Godot;
using System;

public partial class StateMachine : Node
{
	private Node currentState;

	public override void _Ready()
	{
		SwitchToState("Walk"); // État initial
	}

	private void OnStateRequested(string nextState)
	{
		SwitchToState(nextState);
	}

	private void SwitchToState(string stateName)
{
	if (currentState != null)
	{
		if (currentState.HasMethod("OnExit"))
			currentState.Call("OnExit");

		currentState.Disconnect("RequestStateChange", new Callable(this, nameof(OnStateRequested)));
	}

	currentState = GetNode<Node>(stateName); 

	if (currentState.HasSignal("RequestStateChange"))
		currentState.Connect("RequestStateChange", new Callable(this, nameof(OnStateRequested)));

	if (currentState.HasMethod("OnEnter"))
		currentState.Call("OnEnter");

	GD.Print($"🔁 Switched to {stateName}");
}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState != null && currentState.HasMethod("_PhysicsProcess"))
		{
			currentState.Call("_PhysicsProcess", delta);
		}
	}
}
