using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.enemy.EnemyState
{
    public class EnemyIdleState : EnemyStateBase
    {
        public EnemyIdleState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            GD.Print("EnemyIdleState Enter");
        }

        public override void Update(float delta)
        {
            enemy.stateMachine.ChangeState(enemy.stateMachine.patrolState);
            return;
        }

        public override void Exit()
        {
            GD.Print("EnemyIdleState Exit");
        }
    }
}
