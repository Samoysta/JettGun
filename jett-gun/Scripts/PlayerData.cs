using Godot;
using System;

public partial class PlayerData : Node
{
	public bool canFire;
	public bool canDash;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        canFire = true;
		canDash = true;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
