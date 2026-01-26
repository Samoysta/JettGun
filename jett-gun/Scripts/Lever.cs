using Godot;
using System;

public partial class Lever : Node2D
{
	AnimationPlayer anim;
    AnimationPlayer anim2;
    bool opened;
    bool anim2finished;
    PlayerData pd;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        anim2 = GetNode<AnimationPlayer>("Sallanma/InteractSpriteOrigin/AnimationPlayer2");
        pd = GetNode<PlayerData>("/root/PlayerData");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (opened && !anim2finished)
        {
            anim2.PlayBackwards("Start");
            anim2finished = true;
        }
        
    }

	public void Interact()
    {
        if (!opened)
        {
            anim.Play("LeverOpen");
            opened = true;   
            pd.hasJettPack = true;
            pd.canDash = true;
            pd.canFire = true;
            pd.percentage ++;
            pd.Save();
        }
    }

    public void InteractAnimStart()
    {
        if (!opened)
        {
            anim2.Play("Start");            
        }
    }

    public void InteractAnimEnd()
    {
        if (!opened)
        {
            if (anim2.CurrentAnimationPosition == 0)
            {
                anim2.Seek(0.1f, true);
            }
            anim2.PlayBackwards("Start");
        }
    }
}
