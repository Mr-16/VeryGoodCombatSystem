using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.player.PlayerState
{
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(Player player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            GD.Print("进入IdleState");
        }

        public override void Update(float delta)
        {
            Vector2 moveInput = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
            if(moveInput != Vector2.Zero)
            {
                player.ChangeState(player.moveState);
                return;
            }
            if (player.isDodgeColdDown == false && Input.IsActionJustPressed("dodge"))
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
            GD.Print("退出IdleState");
        }
    }
}
