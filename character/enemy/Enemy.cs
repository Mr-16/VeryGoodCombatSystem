using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VeryGoodCombatSystem.character.enemy.EnemyState;

public partial class Enemy : CharacterBase
{
	[Export]
	public float moveSpeed = 100;


	
	
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
    Area2D eyeArea;//视线范围, 目标进入就把chaseTargetPlayer赋值, 离开就置空
    public Player chaseTargetPlayer = null;


    [Export]
	Area2D attackArea;
    [Export]
    public Player attackTargetPlayer = null;


    [Export]
    private EnemyUi ui;

	[Export]
    public int patrolRadius = 1000;//巡逻点生成的范围的半径
	private Vector2 patrolRoot;//巡逻点生成范围的圆心, 值是初始位置
    private Vector2 curTargetPatrolPoint;//当前目标巡逻点




    public EnemyStateMachine stateMachine;

    //不断往玩家方向走, 直到进入攻击范围, 发动攻击, 若玩家出了范围, 继续走
    public override void _Ready()
	{
		base._Ready();
		stateMachine = new EnemyStateMachine(this);//构造函数会初始化各个状态
		curTargetPatrolPoint = GlobalPosition;
		patrolRoot = GlobalPosition;
        eyeArea.BodyEntered += EyeArea_BodyEntered;
        eyeArea.BodyExited += EyeArea_BodyExited;
        attackArea.BodyEntered += AttackArea_BodyEntered;
        attackArea.BodyExited += AttackArea_BodyExited;
    }



    private void AttackArea_BodyEntered(Node2D body)
    {
        if (body is not Player player) return;
        attackTargetPlayer = player;
    }
    private void AttackArea_BodyExited(Node2D body)
    {
        if (body is not Player player) return;
        attackTargetPlayer = null;
    }


    public override void _PhysicsProcess(double delta)
	{
		stateMachine.curState.Update((float)delta);
    }

	
    

    private void EyeArea_BodyEntered(Node2D body)
    {
        if (body is not Player player) return;
        chaseTargetPlayer = player;
        //GD.Print("Player In");
    }
    private void EyeArea_BodyExited(Node2D body)
    {
        if (body is not Player player) return;
        chaseTargetPlayer = null;
        //GD.Print("Player Out");
    }

    public void Patrol()
    {
        if (GlobalPosition.DirectionTo(curTargetPatrolPoint) < new Vector2(0.1f, 0.1f))
        {
            //到了目标巡逻点, 更新目标巡逻点
            Random random = new Random();
            float patrolPointX = random.Next((int)patrolRoot.X - patrolRadius, (int)patrolRoot.X + patrolRadius);
            float patrolPointY = random.Next((int)patrolRoot.Y - patrolRadius, (int)patrolRoot.Y + patrolRadius);
            curTargetPatrolPoint.X = patrolPointX;
            curTargetPatrolPoint.Y = patrolPointY;
        }
        //向巡逻点移动
        Vector2 moveDirection = (curTargetPatrolPoint - GlobalPosition).Normalized();
        Velocity = moveDirection * moveSpeed;
        MoveAndSlide();
    }

    public void Chase()
    {
        if (chaseTargetPlayer == null) return;
        Vector2 moveDirection = (chaseTargetPlayer.GlobalPosition - GlobalPosition).Normalized();
        Velocity = moveDirection * moveSpeed;
        MoveAndSlide();
    }

    public void Attack(float delta)
	{
        attackTimer += delta;
        if (attackTimer < attackDuration) return;

        GD.Print("attacking");
        curOrientation = (attackTargetPlayer.GlobalPosition - GlobalPosition).Normalized();
        switch (curAbility)
        {
            case AbilityType.LongSwordWave:
                LongSwordWave();
                break;
            case AbilityType.ScatterSwordWave:
                ScatterSwordWave();
                break;
            case AbilityType.AllDirectionSwordWave:
                AllDirectionSwordWave();
                break;
            default:
                break;
        }

        attackTimer = 0;
    }

	private void Move()
	{
		if (isBacking || isAttacking) return;
        Vector2 moveDirection = (attackTargetPlayer.GlobalPosition - GlobalPosition).Normalized();
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
