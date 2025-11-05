using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.player.PlayerState
{
    public class PlayerMoveState : PlayerStateBase
    {
        public PlayerMoveState(Player player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            GD.Print("进入MoveState");
        }

        public override void Update(float delta)
        {
            player.Velocity = player.moveDirection * player.moveSpeed;
            player.Anim();
            player.MoveAndSlide();
            player.UpdateCurOrientation();
            player.moveDirection = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
            if (player.moveDirection == Vector2.Zero)
            {
                player.ChangeState(player.idleState);
                return;
            }
            if (Input.IsActionJustPressed("dodge"))
            {
                player.ChangeState(player.dodgeState);
                return;
            }
            if (Input.IsActionJustPressed("attack"))
            {
                player.ChangeState(player.attackState);
                return;
            }
        }

        public override void Exit()
        {
            GD.Print("退出MoveState");
        }
    }
}
