using Godot;
using System;
using VeryGoodCombatSystem.character.player.PlayerState;

public partial class Player : CharacterBase
{
	[Export]
	public float moveSpeed = 300.0f;

    

    [Export]
    public float dodgeDuration = 0.5f;
    public float dodgeTimer = 0f;
    [Export]
    public float dodgeSpeed = 1000f;

    public float beforeAttackDuration = 0.2f;
    public float beforeAttackTimer = 0f;
    public float attackDuration = 0.5f;
    public float attackTimer = 0f;
    public float afterAttackDuration = 0.2f;
    public float afterAttackTimer = 0f;


    [Export]
    public float dodgeColdDownDuration = 0.5f;
    private bool isDodgeColdDown = false;

    public Vector2 moveDirection;
	public Vector2 curOrientation = Vector2.Right;//默认向右, 持久化记录人物朝向

    [Export]
	private Sprite2D body;
    [Export]
    private Node2D curOrientationNode;
    [Export]
    private PlayerUi ui;


    public PlayerStateBase curState;
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerDodgeState dodgeState;
    public PlayerAttackState attackState;

    public override void _Ready()
    {
        base._Ready();
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        dodgeState = new PlayerDodgeState(this);
        attackState = new PlayerAttackState(this);
        ChangeState(idleState);
    }

    public override void _PhysicsProcess(double delta)
	{
        curState.Update((float)delta);
    }

    public void ChangeState(PlayerStateBase newState)
    {
        if (curState != null)
        {
            curState.Exit();
        }
        curState = newState;
        newState.Enter();
    }

    private void HandleIdle()
    {
        GD.Print("HandleIdle()");

    }

    //private void HandleMove()
    //{
    //    GD.Print("HandleMove()");
    //    moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
    //    updateCurOrientation();
    //    Velocity = moveDirection * moveSpeed;
    //    Anim();
    //    MoveAndSlide();

    //    if (moveDirection == Vector2.Zero)
    //    {
    //        SwitchState(PlayerState.Idle);
    //        return;
    //    }
    //    else if (Input.IsActionJustPressed("attack"))
    //    {
    //        SwitchState(PlayerState.BeforeAttack);
    //        return;
    //    }
    //    else if (Input.IsActionJustPressed("dodge"))
    //    {
    //        SwitchState(PlayerState.Dodging);
    //        return;
    //    }

    //}


    //private void HandleBeforeAttack(float delta)
    //{
    //    GD.Print("HandleBeforeAttack()");
    //    //这里如果有按键输入, 可以马上切去move/dodge


    //    beforeAttackTimer += delta;
    //    if(beforeAttackTimer >= beforeAttackDuration)
    //    {
    //        beforeAttackTimer = 0;
    //        Attack();//这里马上攻击, 因为HandleAttack其实只是用来硬直的
    //        SwitchState(PlayerState.Attack);
    //    }
    //}
    //private float attackDuration = 0.5f;
    //private float attackTimer = 0;
    //private void HandleAttack(float delta)
    //{
    //    GD.Print("HandleAttack()");
    //    attackTimer += delta;
    //    if(attackTimer >= attackDuration)
    //    {
    //        attackTimer = 0;
    //        SwitchState(PlayerState.AfterAttack);
    //    }
    //}

    //private void HandleAfterAttack(float delta)
    //{
    //    GD.Print("HandleAfterAttack()");
    //    //这里如果有按键输入, 可以马上切去move/dodge/afterAttack

    //    afterAttackTimer += delta;
    //    if (afterAttackTimer >= afterAttackDuration)
    //    {
    //        attackTimer = 0;
    //        SwitchState(PlayerState.Idle);
    //    }
    //}

    //private void HandleDodge()
    //{
    //    GD.Print("HandleDodge()");
    //    Velocity = curOrientation * dodgeSpeed;
    //    MoveAndSlide();
    //    GetTree().CreateTimer(dodgeDuration).Timeout += () => SwitchState(PlayerState.Idle);
    //}

    private void SwitchState(PlayerStateBase newState)
    {
        if (newState == curState) return;
        GD.Print($"转换状态 : {curState} => {newState}");
        curState = newState;
    }









    //移动控制
 //   private void Move()
	//{
 //       if (isDodging) return;
 //       moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
        
 //   }

	public void UpdateCurOrientation()
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
	public void Anim()
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
        

    }

    ////闪避
    //private void Dodge()
    //{
    //    if (isDodging == true || isDodgeColdDown || !Input.IsActionJustPressed("dodge")) return;//冷却ing或者没按键都不往下走
    //    isDodging = true;

    //    GetTree().CreateTimer(dodgeDuration).Timeout += () =>
    //    {
    //        isDodging = false;
    //        isDodgeColdDown = true;
    //        GetTree().CreateTimer(dodgeColdDownDuration).Timeout += () => isDodgeColdDown = false;
    //    };
    //}

    //private void Dodging(float delta)
    //{
    //    if (isDodging == false) return;
    //    Velocity = curOrientation * dodgeSpeed;
    //    MoveAndSlide();
    //}


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
