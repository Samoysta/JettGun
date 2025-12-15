using Godot;
using System;

public partial class Camera2d : Camera2D
{
	[Export] Node2D target;
	[Export] float speed;
	Vector2 position;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        position.X = Mathf.Lerp(position.X, target.GlobalPosition.X, speed * (float)delta);
		position.Y = Mathf.Lerp(position.Y, target.GlobalPosition.Y, speed * (float)delta);
		//position.Y = target.GlobalPosition.Y;
		GlobalPosition = position;
    }
}
