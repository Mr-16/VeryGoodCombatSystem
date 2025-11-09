using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryGoodCombatSystem.character.player.PlayerState
{
    public abstract class PlayerStateBase
    {
        protected Player player;
        public abstract void Enter();
        public abstract void Update(float delta);
        public abstract void Exit();
    }
}
