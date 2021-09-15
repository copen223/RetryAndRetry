using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BattleModule.BattleStates
{
    // 闲置状态
    class BattleRest:BattleState
    {
        public override void StateStart()
        {
            GameManager.instance.AddListener(GameManager.GameEvent.EnterBattle, OnEnterBattle);
        }

        private void OnEnterBattle()
        {
            ChangeStateTo<BattleInit>();
        }
    }
}
