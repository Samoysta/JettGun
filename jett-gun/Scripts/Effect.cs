using Godot;
using System;

public partial class Effect : Node2D
{
	[Export] float timer1;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        timer1 -= (float)delta;
		if (timer1 < 0)
        {
            QueueFree();
        }
    }
}
