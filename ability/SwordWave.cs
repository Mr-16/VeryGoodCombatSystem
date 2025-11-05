using Godot;
using System;

public partial class SwordWave : Area2D
{
	public int damage;
    public float moveSpeed;
    public Vector2 direction;//飞行方向
    public float surviveTime;


    public override void _Ready()
    {
        BodyEntered += SwardWave_BodyEntered;
        GetTree().CreateTimer(surviveTime).Timeout += () =>
        {
            QueueFree();
        };
	}

    private void SwardWave_BodyEntered(Node2D body)
    {
        if(body is CharacterBase character)
        {
            GD.Print("有人物进入我的攻击范围了");
            character.TakeDamage(damage);

            QueueFree();
        }
    }


    public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += (float)delta * moveSpeed * direction;
        Rotation = direction.Angle();
    }
}
