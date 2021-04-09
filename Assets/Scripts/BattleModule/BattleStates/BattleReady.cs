using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BattleModule.BattleStates
{
    // 战斗准备阶段
    class BattleReady:BattleState
    {
        public override void StateStart()
        {

        }

        private void AddActorIntoBattle(GameObject gb)
        {
            Manager.ActorList.Add(gb);
        }
    }
}
