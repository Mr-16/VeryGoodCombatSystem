using Godot;
using System;
using VeryGoodCombatSystem.character.player.PlayerState;

public partial class Player : CharacterBase
{
	[Export]
	public float moveSpeed = 300.0f;

    [Export]
    public float dodgeSpeed = 1000f;
    private bool isDodging = false;
    [Export]
    private float dodgeDuration = 0.2f;
    private float dodgeTimer = 0f;
    [Export]
    public float dodgeColdDownDuration = 0.5f;
    private bool isDodgeColdDown = false;

    private Vector2 moveDirection;
	private Vector2 curOrientation = Vector2.Right;//默认向右, 持久化记录人物朝向

    [Export]
	private Sprite2D body;
    [Export]
    private Node2D curOrientationNode;
    [Export]
    private PlayerUi ui;

    //private PlayerState curState = PlayerState.Idle;
    
    private enum PlayerState
    {
        Idle,//敌不动, 我不动, 能进入所有状态
        Move,//移动
        Dodging,//翻滚
        Backing,//击退/硬直 : GetBack函数接收时间,方向,力度, 力度为0就可以变成硬直

        BeforeAttack,//攻击前摇
        Attack,//攻击
        AfterAttack,//攻击后摇


        BeforeDefense,//格挡前摇
        Defense,//格挡
        AfterDefense,//格挡后摇
    }

    private PlayerStateBase curState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerDodgeState dodgeState;

    public override void _Ready()
    {
        base._Ready();
        idleState = new PlayerIdleState();
        moveState = new PlayerMoveState()
    }

    public override void _PhysicsProcess(double delta)
	{
        curState.Update();
    }

    private void ChangeState(PlayerStateBase newState)
    {
        if (newState == null) return;
        newState.Enter();
        curState = newState;
        curState.Exit();
    }

    private void HandleIdle()
    {
        GD.Print("HandleIdle()");
        moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
        if(moveDirection != Vector2.Zero)
        {
            SwitchState(PlayerState.Move);
            return;
        }
        else if (Input.IsActionJustPressed("attack"))
        {
            SwitchState(PlayerState.BeforeAttack);
            return;
        }
        else if (Input.IsActionJustPressed("dodge"))
        {
            SwitchState(PlayerState.Dodging);
            return;
        }

    }

    private void HandleMove()
    {
        GD.Print("HandleMove()");
        moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
        updateCurOrientation();
        Velocity = moveDirection * moveSpeed;
        Anim();
        MoveAndSlide();

        if (moveDirection == Vector2.Zero)
        {
            SwitchState(PlayerState.Idle);
            return;
        }
        else if (Input.IsActionJustPressed("attack"))
        {
            SwitchState(PlayerState.BeforeAttack);
            return;
        }
        else if (Input.IsActionJustPressed("dodge"))
        {
            SwitchState(PlayerState.Dodging);
            return;
        }

    }

    private float beforeAttackDuration = 0.1f;
    private float beforeAttackTimer = 0;
    private void HandleBeforeAttack(float delta)
    {
        GD.Print("HandleBeforeAttack()");
        //这里如果有按键输入, 可以马上切去move/dodge


        beforeAttackTimer += delta;
        if(beforeAttackTimer >= beforeAttackDuration)
        {
            beforeAttackTimer = 0;
            Attack();//这里马上攻击, 因为HandleAttack其实只是用来硬直的
            SwitchState(PlayerState.Attack);
        }
    }
    private float attackDuration = 0.5f;
    private float attackTimer = 0;
    private void HandleAttack(float delta)
    {
        GD.Print("HandleAttack()");
        attackTimer += delta;
        if(attackTimer >= attackDuration)
        {
            attackTimer = 0;
            SwitchState(PlayerState.AfterAttack);
        }
    }
    private float afterAttackDuration = 0.1f;
    private float afterAttackTimer = 0;
    private void HandleAfterAttack(float delta)
    {
        GD.Print("HandleAfterAttack()");
        //这里如果有按键输入, 可以马上切去move/dodge/afterAttack

        afterAttackTimer += delta;
        if (afterAttackTimer >= afterAttackDuration)
        {
            attackTimer = 0;
            SwitchState(PlayerState.Idle);
        }
    }

    private void HandleDodge()
    {
        GD.Print("HandleDodge()");
        Velocity = curOrientation * dodgeSpeed;
        MoveAndSlide();
        GetTree().CreateTimer(dodgeDuration).Timeout += () => SwitchState(PlayerState.Idle);
    }

    private void SwitchState(PlayerState newState)
    {
        if (newState == curState) return;
        GD.Print($"转换状态 : {curState} => {newState}");
        curState = newState;
    }









    //移动控制
    private void Move()
	{
        if (isDodging) return;
        moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
        
    }

	private void updateCurOrientation()
	{

        if (moveDirection.X < 0 && moveDirection.Y == 0)//纯左
        {
            curOrientation = Vector2.Left;
            curOrientationNode.Rotation = Mathf.DegToRad(180);
            return;
        }
        if (moveDirection.X > 0 && moveDirection.Y == 0)//纯右
        {
            curOrientation = Vector2.Right;
            curOrientationNode.Rotation = Mathf.DegToRad(0); ;
            return;
        }
        if (moveDirection.X == 0 && moveDirection.Y < 0)//纯上
        {
            curOrientation = Vector2.Up;
            curOrientationNode.Rotation = Mathf.DegToRad(-90);
            return;
        }
        if (moveDirection.X == 0 && moveDirection.Y > 0)//纯下
        {
            curOrientation = Vector2.Down;
            curOrientationNode.Rotation = Mathf.DegToRad(90);
            return;
        }

        if (moveDirection.X < 0 && moveDirection.Y < 0)//左上
        {
            curOrientation = new Vector2(-1, -1);
            curOrientationNode.Rotation = Mathf.DegToRad(-135);
            return;
        }
        if (moveDirection.X < 0 && moveDirection.Y > 0)//左下
        {
            curOrientation = new Vector2(-1, 1);
            curOrientationNode.Rotation = Mathf.DegToRad(135);
            return;
        }
        if (moveDirection.X > 0 && moveDirection.Y < 0)//右上
        {
            curOrientation = new Vector2(1, -1);
            curOrientationNode.Rotation = Mathf.DegToRad(-45);
            return;
        }
        if (moveDirection.X > 0 && moveDirection.Y > 0)//右下
        {
            curOrientation = new Vector2(1, 1);
            curOrientationNode.Rotation = Mathf.DegToRad(45);
            return;
        }
    }

	//动画播放
	private void Anim()
	{
		if(moveDirection.X < 0)
		{
			body.Rotation = -0.2f;

        }
        else if (moveDirection.X > 0)
        {
            body.Rotation = 0.2f;
		}
		else
		{
			body.Rotation = 0;
		}
    }

    //攻击
    private void Attack()
    {
        GD.Print("attacking");
        int waveCount = 5; // 刀波数量（越多越密集）
        float totalAngle = 80f; // 扇形总角度（度），角度越大范围越宽

        // 计算每个刀波的角度偏移（从左到右均匀分布）
        float startAngle = -totalAngle / 2f; // 起始角度（左半部分）
        float angleStep = waveCount > 1 ? totalAngle / (waveCount - 1) : 0f; // 相邻刀波的角度差

        for (int i = 0; i < waveCount; i++)
        {
            // 计算当前刀波的角度（转换为弧度，Godot旋转用弧度）
            float currentAngle = startAngle + i * angleStep;
            float angleRad = currentAngle * Mathf.Pi / 180f;

            // 基于当前朝向旋转，得到该刀波的方向
            Vector2 waveDir = curOrientation.Rotated(angleRad);

            // 实例化刀波并设置属性
            SwordWave wave = swordWaveScene.Instantiate<SwordWave>();
            wave.damage = damage;
            wave.direction = waveDir; // 扇形方向
            wave.moveSpeed = 200;
            wave.surviveTime = 0.5f;
            // 位置：从角色位置沿当前刀波方向偏移50像素（避免贴角色生成）
            wave.GlobalPosition = GlobalPosition + 50 * waveDir;

            GetTree().CurrentScene.AddChild(wave);
        }

    }

    //闪避
    private void Dodge()
    {
        if (isDodging == true || isDodgeColdDown || !Input.IsActionJustPressed("dodge")) return;//冷却ing或者没按键都不往下走
        isDodging = true;

        GetTree().CreateTimer(dodgeDuration).Timeout += () =>
        {
            isDodging = false;
            isDodgeColdDown = true;
            GetTree().CreateTimer(dodgeColdDownDuration).Timeout += () => isDodgeColdDown = false;
        };
    }

    private void Dodging(float delta)
    {
        if (isDodging == false) return;
        Velocity = curOrientation * dodgeSpeed;
        MoveAndSlide();
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        ui.UpdateHealth(curHealth, maxHealth);
    }

    public override void TakeExp(int addExp)
    {
        base.TakeExp(addExp);
        ui.UpdateExpAndLevel(curExp, maxExp, level);
    }

    protected override void RefreshAttribute()
    {
        base.RefreshAttribute();
        ui.UpdateExpAndLevel(curExp, maxExp, level);
        ui.UpdateHealth(maxHealth, curHealth);

    }
}
