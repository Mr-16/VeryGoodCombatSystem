using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.enemy.EnemyState
{
    public abstract class EnemyStateBase
    {
        protected Enemy enemy;
        public abstract void Enter();
        public abstract void Update(float delta);
        public abstract void Exit();
    }
}
