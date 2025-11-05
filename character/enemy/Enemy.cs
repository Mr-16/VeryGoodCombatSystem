using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Enemy : CharacterBase
{
	[Export]
	public float moveSpeed = 100;

	private Player targetPlayer;
	
	
	[Export]
	public float attackColdDown = 3f;


	private bool isAttackColdDown = false;

    private bool isBacking = false;
    private float backDuration = 0.1f;
    private float backTimer = 0;
	private Vector2 backDirection;
	private float backStrength;

    private bool isAttacking = false;
	private float beforeAttackDuration = 0.5f;
	private float afferAttackDuration = 0.5f;

    [Export]
	ColorRect rect;
	[Export]
	Area2D damageArea;
    [Export]
    private EnemyUi ui;


    //不断往玩家方向走, 直到进入攻击范围, 发动攻击, 若玩家出了范围, 继续走
    public override void _Ready()
	{
		base._Ready();
        targetPlayer = (Player)GetTree().GetNodesInGroup("player")[0];
    }

    public override void _PhysicsProcess(double delta)
	{

        Back();
        Move();
        Attack();
    }


	private void Attack()
	{
        if (isAttackColdDown || isAttacking) return;
        
        List<Node2D> bodyList = damageArea.GetOverlappingBodies().ToList();
		foreach(Node2D body in bodyList)
		{
			if (body is not Player) continue;
			

			isAttacking = true;
			GetTree().CreateTimer(beforeAttackDuration).Timeout += () =>//前摇
			{
                Vector2 targetDirection = (targetPlayer.GlobalPosition - GlobalPosition).Normalized();
                SwordWave wave = swordWaveScene.Instantiate<SwordWave>();
				wave.damage = damage / 10;
				wave.direction = targetDirection;
				wave.moveSpeed = 100;
				wave.surviveTime = 5f;
				wave.GlobalPosition = GlobalPosition + 60 * targetDirection;
				GetTree().CurrentScene.AddChild(wave);

				GetTree().CreateTimer(attackColdDown).Timeout += () => isAttackColdDown = false;
				isAttackColdDown = true;
				GetTree().CreateTimer(afferAttackDuration).Timeout += () => isAttacking = false;

            };

			break;
        }
    }

	private void Move()
	{
		if (isBacking || isAttacking) return;
        Vector2 moveDirection = (targetPlayer.GlobalPosition - GlobalPosition).Normalized();
		Velocity = moveDirection * moveSpeed;
		MoveAndSlide();
	}

    public override void TakeDamage(int damage)
    {
		base.TakeDamage(damage);
        rect.Color = new Color("#880404"); // 立即变红
        GetTree().CreateTimer(0.2).Timeout += () =>
        {
            rect.Color = new Color("White");
        };
		ui.UpdateHealth(curHealth, maxHealth);
    }

	protected override void Die()
	{
		base.Die();
		GD.Print("Enemy : 又收皮~~");
	}

	public void GetBack(float backStrength, Vector2 backDirection)
	{
		this.backDirection = backDirection;
		this.backStrength = backStrength;
		isBacking = true;
    }
	private void Back()
	{
		if (isBacking == false) return;
		GetTree().CreateTimer(backDuration).Timeout += () => isBacking = false;
		Velocity = backDirection * backStrength;
		MoveAndSlide();
    }

    protected override void RefreshAttribute()
    {
        base.RefreshAttribute();
        ui.UpdateLevel(level);
		ui.UpdateHealth(curHealth, maxHealth);
    }

   
}
