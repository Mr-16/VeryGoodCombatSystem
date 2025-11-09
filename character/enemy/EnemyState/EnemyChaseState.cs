using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.enemy.EnemyState
{
    public class EnemyChaseState : EnemyStateBase
    {
        public EnemyChaseState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            GD.Print("EnemyPatrolState Enter");
        }

        public override void Update(float delta)
        {
            if(enemy.chaseTargetPlayer == null)
            {
                enemy.stateMachine.ChangeState(enemy.stateMachine.idleState);
                return;
            }
            if (enemy.attackTargetPlayer != null)
            {
                enemy.stateMachine.ChangeState(enemy.stateMachine.attackState);
                return;
            }
            enemy.Chase();
        }

        public override void Exit()
        {
            GD.Print("EnemyPatrolState Exit");
        }
    }
}
