using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ExpBall : RigidBody2D
{
	[Export]
	public int exp = 0;

    [Export]
    public float moveSpeed = 300;

    [Export]
	Area2D chaseArea;
    CharacterBase target;

    [Export]
    Area2D captureArea;//捕获范围, 进入范围的target被捕获, 调他的加经验

    public override void _Ready()
	{
        chaseArea.BodyEntered += ChaseArea_BodyEntered;
        chaseArea.BodyExited += ChaseArea_BodyExited;
    }

    private void ChaseArea_BodyEntered(Node2D body)
    {
        if (body is not CharacterBase character) return;
        target = character;
    }
    private void ChaseArea_BodyExited(Node2D body)
    {
        if (body is not CharacterBase character) return;
        target = null;
    }

    public override void _PhysicsProcess(double delta)
	{
        if (target == null) return;

        if (GlobalPosition.DistanceTo(target.GlobalPosition) < 50)
        {
            target.TakeExp(exp);
            QueueFree();
        }

        Vector2 moveDir = (target.GlobalPosition - GlobalPosition).Normalized();
        Position += moveDir * (float)delta * moveSpeed;
    }
}
