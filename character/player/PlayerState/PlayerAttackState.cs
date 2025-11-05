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
                Vector2 waveDir = player.curOrientation.Rotated(angleRad);

                // 实例化刀波并设置属性
                SwordWave wave = player.swordWaveScene.Instantiate<SwordWave>();
                wave.damage = player.damage;
                wave.direction = waveDir; // 扇形方向
                wave.moveSpeed = 300;
                wave.surviveTime = 0.3f;
                // 位置：从角色位置沿当前刀波方向偏移50像素（避免贴角色生成）
                wave.GlobalPosition = player.GlobalPosition + 50 * waveDir;

                player.GetTree().CurrentScene.AddChild(wave);
            }
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
