using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleTurnEnd : BattleState
    {
        public override void StateStart()
        {
            Manager.EventInvokeByState(BattleManager.BattleEvent.TurnEnd);
            Manager.CurActorIndex += 1;
            if (Manager.CurActorIndex >= Manager.ActorQueue.Count) Manager.CurActorIndex = 0;
            ChangeStateTo<BattleTurnStart>();
            // 一轮结束
        }
    }
}
