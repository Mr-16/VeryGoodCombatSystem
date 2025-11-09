using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.enemy.EnemyState
{
    public class EnemyStateMachine
    {
        public EnemyIdleState idleState;//待机
        public EnemyPatrolState patrolState;//巡逻
        public EnemyChaseState chaseState;//追逐
        public EnemyAttackState attackState;//攻击

        public EnemyStateMachine(Enemy enemy)
        {
            idleState = new EnemyIdleState(enemy);
            patrolState = new EnemyPatrolState(enemy);
            chaseState = new EnemyChaseState(enemy);
            attackState = new EnemyAttackState(enemy);
            ChangeState(idleState);
        }

        public EnemyStateBase curState;
        public void ChangeState(EnemyStateBase newState)
        {
            if (curState != null) curState.Exit();
            curState = newState;
            curState.Enter();
        }
    }
}
