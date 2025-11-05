using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ExpBall : RigidBody2D
{
	[Export]
	public int exp = 100;

    [Export]
    public float moveSpeed = 300;

    [Export]
    float captureDistance = 50;//捕获范围, 进入范围的target被捕获, 调他的加经验

    CharacterBase target;
	Area2D area;
    Timer checkTimer; // 目标扫描定时器



    public override void _Ready()
	{
		area = GetNode<Area2D>("Area2D");
        checkTimer = new Timer();
        checkTimer.WaitTime = 0.5f; // 0.5秒间隔
        checkTimer.OneShot = false; // 循环触发
        checkTimer.Timeout += CheckTarget; // 连接超时信号到CheckTarget方法
        AddChild(checkTimer);
        checkTimer.Start(); // 启动定时器
    }

	public override void _PhysicsProcess(double delta)
	{
		if (target != null)
		{
            Vector2 moveDistance = (target.GlobalPosition - GlobalPosition).Normalized();
            Position += moveDistance * (float)delta * moveSpeed;

            //检查一下target的距离, 如果够近就捕获了
            if (this.GlobalPosition.DistanceTo(target.GlobalPosition) < captureDistance)
            {
                target.TakeExp(exp);
                //GD.Print("捕获!");
                this.QueueFree();
            }
        }

    }

	private void CheckTarget()
	{
		List<Node2D> bodyList =  area.GetOverlappingBodies().ToList();
		CharacterBase targetCharacter = null;
		float minDistance = float.MaxValue;
        foreach (Node2D body in bodyList)
		{
			if (body is not CharacterBase curCharacter) continue;
			float curDistance = GlobalPosition.DistanceTo(curCharacter.GlobalPosition);
			if (curDistance < minDistance)
			{
				minDistance = curDistance;
                targetCharacter = curCharacter;
            }
        }

		if (targetCharacter == null)
		{
			target = null;
			return;
		}
		target = targetCharacter;
	}
}
