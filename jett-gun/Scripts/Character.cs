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
    [Export] float dashSpeed;
    [Export] float dashDur;
    [Export] float dashCD;
    [Export] PackedScene dashDust;
    [Export] PackedScene dashEffect;
    [Export] Node2D staminaBar;
    [Export] float stamina;
    float currentStamina;
    int maxDashAmount = 1;
    float dashDuration;
    float dashCoolDown;
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
    Timer dashEffectCD;
    AnimationPlayer anim;
    Sprite2D JettPack;
    CpuParticles2D jettPackEffect;
    bool canJett;
    public Queue<Bullet1> bullets = new();

    public override void _Ready()
    {
        currentStamina = stamina;
        ctimer = coyotoTimer;
        playerData = GetNode<PlayerData>("/root/PlayerData");
        characterSprite = GetNode<Node2D>("CharacterHitBox/Sprite/Sprite2/CharacterSprite");
        foot = characterSprite.GetNode<Node2D>("Foot");
        spriteUp = characterSprite.GetNode<AnimatedSprite2D>("Up");
        spriteDown = characterSprite.GetNode<AnimatedSprite2D>("Down");
        gunCoolDown = GetNode<Timer>("GunCoolDown");
        spriteWithGun = characterSprite.GetNode<AnimatedSprite2D>("Gun");
        dashEffectCD = GetNode<Timer>("DashEffectCD");
        GunUpPos = spriteWithGun.GetNode<Node2D>("GunUpPos");
        GunDownPos = spriteWithGun.GetNode<Node2D>("GunDownPos");
        JettPack = characterSprite.GetNode<Sprite2D>("JettPack");
        JettPack.Visible = false;
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
        if (playerData.hasJettPack) JettPack.Visible = true; 
        jettPackEffect = JettPack.GetNode<CpuParticles2D>("JettPackParticles");
    }

    public override void _Process(double delta)
    {
        //StaminaBar
        Vector2 targetPos = new Vector2((616 / stamina * currentStamina) - 616, 0);
        if (staminaBar.Position >= targetPos)
        {
            staminaBar.Position = targetPos;
        }
        else
        {
            staminaBar.Position = staminaBar.Position.Lerp(targetPos, 5 * (float)delta);   
        }
        //DashEffect Bölümü
        if (dashDuration > 0)
        {
            if (dashEffectCD.IsStopped())
            {
                dashEffectCD.Start();
                if (characterSprite.Scale.Y > 0)
                {
                    AnimatedSpriteSpawn(dashEffect, characterSprite.GlobalPosition, false, new Vector2(1,1), characterSprite.GlobalRotationDegrees);
                }
                else
                {
                    AnimatedSpriteSpawn(dashEffect, characterSprite.GlobalPosition, false, new Vector2(1,-1), characterSprite.GlobalRotationDegrees);
                }
            }
        }
        if (IsOnFloor())
        {
            if (maxDashAmount == 0)
            {
                maxDashAmount++;
            }
        }
        //Ateş etme bölümü
        if (playerData.canFire)
        {
            if (Input.IsActionPressed("X") && dashDuration <= 0)
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
        if (Input.IsActionJustPressed("Down") && IsOnFloor() && dashDuration <= 0)
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
		if (Interact.Count > 0 && !IsOnFloor() && dashDuration <= 0)
        {
            if (Interact[0].GetParent().HasMethod("InteractAnimEnd") && !calledInteract1)
            {
                Interact[0].GetParent().Call("InteractAnimEnd");
				calledInteract1 = true;
            }
        }
		else if (Interact.Count > 0 && IsOnFloor() && dashDuration <= 0)
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
        // Düşme Animasyonu
		if (!IsOnFloor() && dashDuration <= 0)
        {
            if (velocity.Y > 0)
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
            if (Velocity.Y <= -JumpVelocity)
            {
                anim.Play("Jump");
                anim.Seek(0);
            }
            if (Velocity.Y < 0)
            {
                if (spriteUp.Animation != "Jump")
                {
                    spriteUp.Play("Jump");
                }
                if (spriteDown.Animation != "Jump")
                {
                    spriteDown.Play("Jump");
                }
                if (Velocity.Y < -JumpVelocity)
                {
                    spriteUp.Frame = 0;
                    spriteDown.Frame = 0;
                }
                
            }
        }
        //JettPack kısmı
        if (ctimer <= 0 && !isJumping)
        {
            if (Input.IsActionPressed("Z") && playerData.hasJettPack && canJett && dashDuration <= 0 && currentStamina > 0)
            {
                if (!jettPackEffect.Emitting) jettPackEffect.Emitting = true;
                if (Velocity.Y != 0)
                {
                    spriteDown.Frame = 0;
                    spriteUp.Frame = 0;    
                }
                velocity.Y += -10000 * (float)delta;
                currentStamina -= 50 * (float)delta;
                velocity.Y = Mathf.Clamp(velocity.Y, -800, 1400);
            }
            else
            {
                if (jettPackEffect.Emitting) jettPackEffect.Emitting = false;
            }
        }
        else
        {
            if (jettPackEffect.Emitting) jettPackEffect.Emitting = false;
        }
        //dash Timer kısmı
        if (dashDuration > 0)
        {
            if (canJett) canJett = false;
            if (!Input.IsActionPressed("Z"))
            {
                canJett = true;
            }
            dashDuration -= (float)delta;
        }
        if (dashCoolDown > 0)
        {
            dashCoolDown -= (float)delta;
        }
        //Daha iyi zıplama timer kısmı
        if (IsOnFloor())
        {
            ctimer = coyotoTimer;
        }
        if (dashDuration > 0)
        {
            ctimer = 0;
        }
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
            if (dashDuration > 0)
            {
                isJumping = false;
            }
        }
		// Add the gravity.
		if (!IsOnFloor() && velocity.Y < 1400 && dashDuration <= 0)
		{
			velocity += gravity * GetGravity() * (float)delta;
		}
		// Tavana  ve Yere Çarpınca Y eksenindeki hızı sıfırlama
        if (IsOnCeiling() && velocity.Y < 0)
        {
            velocity.Y = 0;
            isJumping = false;
        }
        //Yere Çarpma
		if (IsOnFloor() && velocity.Y > 0)
        {
            currentStamina = stamina;
            canJett = false;
            anim.Play("Fall");
            anim.Seek(0);
            AnimatedSpriteSpawn(FallEffect,foot.GlobalPosition, false, new Vector2(1,1), 0);
            ctimer = coyotoTimer;
            velocity.Y = 0;
            if (Velocity.X < -Speed || Velocity.X > Speed)
            {
                velocity = Vector2.Zero;
                Velocity = Vector2.Zero;
            }
        }
		// Movement X
		if (Input.IsActionPressed("Right") != Input.IsActionPressed("Left") && dashDuration <= 0)
        {
            if (Input.IsActionPressed("Right") && velocity.X <= Speed)
			{
				velocity.X = Speed;
				if (characterSprite.Scale.Y < 0)
                {
                    characterSprite.Scale *= new Vector2(1,-1);
                    characterSprite.RotationDegrees = 0;
                }
			}
			else if (Input.IsActionPressed("Left") && velocity.X >= -Speed)
			{
				velocity.X = -Speed;
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
            if (dashDuration <= 0)
            {
                if (velocity.X >= -Speed && velocity.X <= Speed)
                {
                    velocity.X = 0;
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
        }
		// Jump
		if (Input.IsActionJustPressed("Z") && ctimer > 0 && dashDuration <= 0)
		{
            AnimatedSpriteSpawn(JumpEffect,foot.GlobalPosition, false, new Vector2(1,1), 0);
            isJumping = true;
            canJett = false;
            jTimer = jumpTimer;
            ctimer = 0;
		}
        if (!Input.IsActionPressed("Z") && !IsOnFloor())
        {
            if (!canJett)
            {
                canJett = true;
            }
        }
        if (isJumping && dashDuration <= 0)
        {
            if (Input.IsActionJustReleased("Z"))
            {
                isJumping = false;
            }
            velocity.Y = -JumpVelocity;
        }
        if (Input.IsActionJustPressed("C"))
        {
            if (playerData.canDash && maxDashAmount > 0)
            {
                if (dashCoolDown <= 0)
                {
                    dashCoolDown = dashCD;
                    dashDuration = dashDur;
                    maxDashAmount--;
                    spriteDown.Play("Dash");
                    spriteUp.Play("Dash");
                    anim.Play("RESET");
                    if (characterSprite.Scale.Y < 0)
                    {
                        AnimatedSpriteSpawn(dashDust, GlobalPosition, false, new Vector2(1,1), 180);
                    }
                    else
                    {
                        AnimatedSpriteSpawn(dashDust, GlobalPosition, false, new Vector2(1,1), 0);
                    }
                    if (!spriteUp.Visible)
                    {
                        spriteUp.Visible = true;
                    }
                    if (spriteWithGun.Visible)
                    {
                        spriteWithGun.Visible = false;
                    }
                }
            }
        }
        if (dashDuration > 0)
        {
            if (velocity.X != 0)
            {
                Velocity = new Vector2(velocity.X / Mathf.Abs(velocity.X) * dashSpeed, 0);
            }
            else
            {
                if (characterSprite.Scale.Y < 0)
                {
                    Velocity = new Vector2(-dashSpeed, 0);
                }
                else
                {
                    Velocity = new Vector2(dashSpeed, 0);
                }
            }
            velocity.Y = 0;
        }
        else
        {
            if (Velocity.X < -Speed || Velocity.X > Speed)
            {
                velocity = Vector2.Zero;
                Velocity = Vector2.Zero;
            }
            Velocity = new Vector2(Mathf.MoveToward(Velocity.X, velocity.X, accel * (float)delta), velocity.Y);   
        }
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
