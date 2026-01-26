using Godot;
using System;

public partial class Effect : Node2D
{
	[Export] float timer1;
	float timer;
	[Export] AnimatedSprite2D sprite;
	[Export] AnimationPlayer anim;
	public bool canUse;
	Character character;
	public bool isOff;
	public override void _Ready()
	{
		timer = timer1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (timer > 0)
		{
			timer -= (float)delta;
		}
		else
		{
			if (IsInGroup("Trash"))
			{
				QueueFree();
			}
			setOff();
		}
    }
	public void Init(Character node)
	{
		character = node;
	}
	public void Play()
	{
		timer = timer1;
		if (IsInGroup("Halo"))
		{
			anim.Play("Start");
		}
		else
		{
			sprite.Play();
		}
	}
	public void setOff()
	{
		if (!isOff)
		{
			Visible = false;
			if (IsInGroup("Jump"))
			{
				character.jumpEffects.Enqueue(this);	
				sprite.Frame = 0;
			}
			else if (IsInGroup("Fall"))
			{
				character.fallEffects.Enqueue(this);
				sprite.Frame = 0;	
			}
			else if (IsInGroup("Dash"))
			{
				character.dashEffects.Enqueue(this);
				sprite.Frame = 0;	
			}
			else if (IsInGroup("Halo"))
			{
				character.haloEffects.Enqueue(this);
				anim.Seek(0);
			}
			else if (IsInGroup("Fire"))
			{
				character.fireEffects.Enqueue(this);
				sprite.Frame = 0;
			}
			isOff = true;	
		}
	}
}
