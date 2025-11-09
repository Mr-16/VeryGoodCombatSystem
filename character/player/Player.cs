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






    [Export]
    public float dodgeColdDownDuration = 0.5f;
    public bool isDodgeColdDown = false;

    public Vector2 moveDirection;

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

 
    public void Move()
    {
        moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
        Velocity = moveDirection * moveSpeed;
        UpdateCurOrientation();
        Anim();
        MoveAndSlide();
    }

	private void UpdateCurOrientation()
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


    public void Dodge(float delta)
    {
        Velocity = curOrientation * dodgeSpeed;
        MoveAndSlide();
    }


    //攻击
    public void Attack()
    {
        GD.Print("attacking");
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


    private void SwordWave()
    {
        
    }
}
