using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

public partial class Character : CharacterBody2D
{
    PlayerData playerData;
	[Export] float Speed;
	[Export] float JumpVelocity;
	[Export] float accel;
	[Export] float gravity;
    [Export] PackedScene JumpEffect;
    [Export] PackedScene FallEffect;
    [Export] PackedScene bullet1;
    [Export] PackedScene fireEffect1;
    [Export] float jumpTimer;
    [Export] float coyotoTimer;
    float jTimer;
    bool isJumping;
    float ctimer;
	List<Node2D> Interact = new List<Node2D>();
	Node2D characterSprite;
    AnimatedSprite2D spriteUp;
    AnimatedSprite2D spriteWithGun;
    AnimatedSprite2D spriteDown;
    Node2D GunUpPos;
    Node2D GunDownPos;
    Node2D GunPos;
	bool calledInteract1 = true;
	Vector2 velocity;
    Node2D foot;
    Timer gunCoolDown;
    AnimationPlayer anim;
    public Queue<Bullet1> bullets = new();

    public override void _Ready()
    {
        ctimer = coyotoTimer;
        playerData = GetNode<PlayerData>("/root/PlayerData");
        characterSprite = GetNode<Node2D>("CharacterHitBox/Sprite/Sprite2/CharacterSprite");
        foot = characterSprite.GetNode<Node2D>("Foot");
        spriteUp = characterSprite.GetNode<AnimatedSprite2D>("Up");
        spriteDown = characterSprite.GetNode<AnimatedSprite2D>("Down");
        gunCoolDown = GetNode<Timer>("GunCoolDown");
        spriteWithGun = characterSprite.GetNode<AnimatedSprite2D>("Gun");
        GunUpPos = spriteWithGun.GetNode<Node2D>("GunUpPos");
        GunDownPos = spriteWithGun.GetNode<Node2D>("GunDownPos");
        GunPos = spriteWithGun.GetNode<Node2D>("GunPos");
        anim = characterSprite.GetNode<AnimationPlayer>("AnimationPlayer");
        for (int i = 0; i < 15; i++)
        {
            Bullet1 Bullet = (Bullet1)bullet1.Instantiate();  
            Bullet.SetProcess(false);
            Bullet.SetPhysicsProcess(false);
            Bullet.Visible = false;
            GetParent().CallDeferred("add_child", Bullet);
            Bullet.Call("Init", this);
            bullets.Enqueue(Bullet);
        }
    }

    public override void _Process(double delta)
    {
        //Ateş etme bölümü
        if (playerData.canFire)
        {
            if (Input.IsActionPressed("X"))
            {
                if (spriteUp.Visible)
                {
                    spriteUp.Visible = false;
                }
                if (!spriteWithGun.Visible)
                {
                    spriteWithGun.Visible = true;
                }
                if (Input.IsActionPressed("Up"))
                {
                    spriteWithGun.Play("GunUp");
                    if (gunCoolDown.IsStopped())
                    {
                        SpawnBullet(-90, GunUpPos.GlobalPosition + new Vector2(0,-48), new Vector2(0,-1));
                        AnimatedSpriteSpawn(fireEffect1, GunUpPos.GlobalPosition, true, new Vector2(1,1), -90);
                    }
                }
                else if (Input.IsActionPressed("Down") && !IsOnFloor())
                {
                    spriteWithGun.Play("GunDown");
                    if (gunCoolDown.IsStopped())
                    {
                        SpawnBullet(90, GunDownPos.GlobalPosition + new Vector2(0,40), new Vector2(0,1));
                        AnimatedSpriteSpawn(fireEffect1, GunDownPos.GlobalPosition, true, new Vector2(1,1), 90);
                    }
                }
                else
                {
                    spriteWithGun.Play("Gun");
                    if (gunCoolDown.IsStopped())
                    {
                        if (characterSprite.Scale.Y > 0)
                        {
                            SpawnBullet(0, GunPos.GlobalPosition + new Vector2(44,0), new Vector2(1,0));
                        }
                        else
                        {
                            SpawnBullet(-180, GunPos.GlobalPosition + new Vector2(-44,0), new Vector2(-1,0));
                        }
                        if (characterSprite.GlobalScale.Y < 0)
                        {
                            AnimatedSpriteSpawn(fireEffect1, GunPos.GlobalPosition, true, new Vector2(1,-1), 180);   
                        }
                        else
                        {
                            AnimatedSpriteSpawn(fireEffect1, GunPos.GlobalPosition, true, new Vector2(1,1), 0);
                        }
                    }
                }
                if (gunCoolDown.IsStopped())
                {
                    gunCoolDown.Start();
                }
            }
        }
        if (gunCoolDown.IsStopped())
        {
            if (!spriteUp.Visible)
            {
                spriteUp.Visible = true;
            }
            if (spriteWithGun.Visible)
            {
                spriteWithGun.Visible = false;
            }
        }
        //Interact Yapma
        if (Input.IsActionJustPressed("Down") && IsOnFloor())
        {
            if (Interact.Count > 0)
            {
                if (Interact[0].GetParent().HasMethod("Interact"))
                {
                    Interact[0].GetParent().Call("Interact");
                }
            }
        }
        //Interact Animasyon Yapma
		if (Interact.Count > 0 && !IsOnFloor())
        {
            if (Interact[0].GetParent().HasMethod("InteractAnimEnd") && !calledInteract1)
            {
                Interact[0].GetParent().Call("InteractAnimEnd");
				calledInteract1 = true;
            }
        }
		else if (Interact.Count > 0 && IsOnFloor())
        {
            if (Interact[0].GetParent().HasMethod("InteractAnimStart") && calledInteract1)
            {
                Interact[0].GetParent().Call("InteractAnimStart");
				calledInteract1 = false;
            }
        }
    }


	public override void _PhysicsProcess(double delta)
	{
        velocity.X = Velocity.X;
        //Daha iyi zıplama kısmı
        if (velocity.Y > 0)
        {
            if (ctimer > 0)
            {
                ctimer -= (float)delta;
            }
        }
        if (isJumping)
        {
            if (jTimer > 0)
            {
                jTimer -= (float)delta;
            }
            else
            {
                isJumping = false;
            }
        }
		// Add the gravity.
		if (!IsOnFloor() && velocity.Y < 5000)
		{
			velocity += gravity * GetGravity() * (float)delta;
		}
		// Tavana  ve Yere Çarpınca Y eksenindeki hızı sıfırlama
        if (IsOnCeiling() && velocity.Y < 0)
        {
            velocity.Y = 0;
            isJumping = false;
        }
		if (IsOnFloor() && velocity.Y > 0)
        {
            anim.Play("Fall");
            AnimatedSpriteSpawn(FallEffect,foot.GlobalPosition, false, new Vector2(1,1), 0);
            ctimer = coyotoTimer;
            velocity.Y = 0;
        }
		// Movement X
		if (Input.IsActionPressed("Right") != Input.IsActionPressed("Left"))
        {
            if (Input.IsActionPressed("Right") && velocity.X <= Speed)
			{
				velocity.X = Mathf.MoveToward(velocity.X, Speed, accel * (float)delta);
				if (characterSprite.Scale.Y < 0)
                {
                    characterSprite.Scale *= new Vector2(1,-1);
                    characterSprite.RotationDegrees = 0;
                }
			}
			else if (Input.IsActionPressed("Left") && velocity.X >= -Speed)
			{
				velocity.X = Mathf.MoveToward(velocity.X, -Speed, accel * (float)delta);
				if (characterSprite.Scale.Y > 0)
                {
                    characterSprite.Scale *= new Vector2(1,-1);
                    characterSprite.RotationDegrees = -180;
                }
			}
			if (IsOnFloor())
            {
                if (spriteDown.Animation != "Run")
                {
                    spriteDown.Play("Run");
                }
                if (spriteUp.Animation != "Run")
                {
                    spriteUp.Play("Run");
                }
            }
        }    
		else
        {
            if (velocity.X >= -Speed && velocity.X <= Speed)
            {
                velocity.X = Mathf.MoveToward(velocity.X, 0, accel * (float)delta);
            }
			if (IsOnFloor())
            {
                if (spriteUp.Animation != "Idle")
                {
                    spriteUp.Play("Idle");
                }
                if (spriteDown.Animation != "Idle")
                {
                    spriteDown.Play("Idle");
                }
            }
        }
		// Jump
		if (Input.IsActionJustPressed("Z") && ctimer > 0)
		{
            spriteUp.Play("Jump");
            spriteDown.Play("Jump");
            AnimatedSpriteSpawn(JumpEffect,foot.GlobalPosition, false, new Vector2(1,1), 0);
            isJumping = true;
            jTimer = jumpTimer;
            ctimer = 0;
		}
        if (isJumping)
        {
            if (Input.IsActionJustReleased("Z"))
            {
                isJumping = false;
            }
            velocity.Y = -JumpVelocity;
        }
		// Düşme Animasyonu
		if (velocity.Y > 0 && !IsOnFloor())
        {
            if (spriteUp.Animation != "Fall")
            {
                spriteUp.Play("Fall");
            }
            if (spriteDown.Animation != "Fall")
            {
                spriteDown.Play("Fall");
            }
        }
        //esnek Zıplama
        if (velocity.Y <= -JumpVelocity && !IsOnFloor())
        {
            anim.Play("Jump");
            anim.Seek(0);
        }
		Velocity = velocity;
		MoveAndSlide();
    }

	public void InteractEntered(Node2D body)
    {
        if (body.IsInGroup("Interactable"))
        {
            Interact.Add(body);
        }
    }

	public void InteractExited(Node2D body)
    {
        if (body.IsInGroup("Interactable"))
        {
            Interact.Remove(body);
            if (body.GetParent().HasMethod("InteractAnimEnd") && !calledInteract1)
            {
                body.GetParent().Call("InteractAnimEnd");
				calledInteract1 = true;
            }
        }
    }
    void AnimatedSpriteSpawn(PackedScene node,Vector2 pos, bool AddChild, Vector2 scale, float rot)
    {
        Node2D effect = (Node2D)node.Instantiate();
        effect.GlobalPosition = pos;
        effect.GlobalScale *= scale;
        effect.GlobalRotationDegrees = rot;
        if (AddChild)
        {
            GetParent().AddChild(effect);
            effect.Reparent(this);
        }
        else
        {
            GetParent().AddChild(effect);
        }
    }
    void SpawnBullet(float rotation, Vector2 position, Vector2 dir)
    {
        Bullet1 bul = bullets.Dequeue();
        bul.isOff = false;
        bul.SetProcess(true);
        bul.SetPhysicsProcess(true);
        bul.Visible = true;
        bul.GlobalRotationDegrees = rotation;
        bul.GlobalPosition = position;
        bul.dir = dir;
    }
}
