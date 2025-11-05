using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.player.PlayerState
{
    public class PlayerDodgeState : PlayerStateBase
    {
        public PlayerDodgeState(Player player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            GD.Print("进入DodgeState");
        }

        public override void Update(float delta)
        {
            player.dodgeTimer += delta;
            if (player.dodgeTimer >= player.dodgeDuration)
            {
                player.dodgeTimer = 0;
                player.ChangeState(player.idleState);
            }
            player.Dodge();

        }

        public override void Exit()
        {
            GD.Print("退出DodgeState");
            player.isDodgeColdDown = true;
            player.GetTree().CreateTimer(player.dodgeColdDownDuration).Timeout += ()=> player.isDodgeColdDown = false;
        }
    }
}
