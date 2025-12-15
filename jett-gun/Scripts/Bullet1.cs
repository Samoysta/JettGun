using Godot;
using System;

public partial class Bullet1 : CharacterBody2D
{
	[Export] int damage = 1;
    public bool canGo;
    public Vector2 dir;
	[Export] float speed;
    [Export] PackedScene smokeEffect1;
    RandomNumberGenerator rand = new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        rand.Randomize();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
    {
        Velocity = speed * dir;
        var colliderInfo = MoveAndCollide(Velocity * (float)delta);

        if (colliderInfo != null)
        {
            Node2D body = (Node2D)colliderInfo.GetCollider();
            Vector2 hitPos = colliderInfo.GetPosition();
            if (body.Name != "Character")
            {
                if (body.HasMethod("TakeDamage"))
                {
                    body.Call("TakeDamage", damage);
                }
                SpawnSmoke(smokeEffect1, hitPos);
                setOff();
            }
        }
    }

    
	void SpawnSmoke(PackedScene effect, Vector2 pos)
    {
        Node2D Effect = (Node2D)effect.Instantiate();
        Effect.GlobalPosition = pos;
        Effect.GlobalRotationDegrees = rand.RandiRange(-180, 180);
        GetParent().AddChild(Effect);
    }
    void ScreenExited()
    {
        setOff();
    }
    void setOff()
    {
        Visible = false;
        SetProcess(false);
        SetPhysicsProcess(false);
        canGo = true;
    }
}
