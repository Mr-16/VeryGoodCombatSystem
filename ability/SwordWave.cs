using Godot;
using System;

public partial class SwordWave : Area2D
{
    public CharacterBase parent;
	public int damage;
    public float moveSpeed;
    public Vector2 direction;//飞行方向
    public float surviveTime;


    public override void _Ready()
    {
        BodyEntered += SwardWave_BodyEntered;
        AreaEntered += SwordWave_AreaEntered;
        GetTree().CreateTimer(surviveTime).Timeout += () =>
        {
            QueueFree();
        };
	}

    private void SwordWave_AreaEntered(Area2D area)
    {
        if (area is not SwordWave wave) return;
        if (wave.parent == parent) return;
        GD.Print("有其他剑进入我的攻击范围了");
        QueueFree();//有其他剑进入我的攻击范围了

    }

    private void SwardWave_BodyEntered(Node2D body)
    {
        if (body is not CharacterBase character) return;
        if (character == parent) return;
        GD.Print("有其他人物进入我的攻击范围了");
        character.TakeDamage(damage);
        QueueFree();

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
