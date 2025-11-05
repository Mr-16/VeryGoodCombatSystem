using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace VeryGoodCombatSystem.character.player.PlayerState
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(Player player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            GD.Print("PlayerAttackState Enter");
            //按技能给施法时间赋值
            player.attackDuration = 0.3f;


            player.Attack();
        }

        public override void Update(float delta)
        {
            player.attackTimer += delta;
            if (player.attackTimer < player.attackDuration) return;

            player.attackTimer = 0;
            player.ChangeState(player.idleState);
            return;
        }

        public override void Exit()
        {
            GD.Print("PlayerAttackState Exit");
        }
    }
}
