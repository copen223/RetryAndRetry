using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleStart : BattleState
    {
        public override void StateStart()
        {
            // 排列对象
            Manager.ActorQueue = new List<GameObject>(Manager.ActorList.ToArray());
            Manager.ActorQueue.Sort(new ActorSortByAdvantage());
            Manager.EventInvokeByState(BattleManager.BattleEvent.ActorQueueCountChange);
            Manager.CurActorIndex = 0;
            // 切换至战斗开始
            ChangeStateTo<BattleTurnStart>();
        }

        public override void StateUpdate()
        {
            StateStart();
        }

        // 排序方法
        private class ActorSortByAdvantage : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                if (x.GetComponent<ActorController>().advantage > y.GetComponent<ActorController>().advantage)
                    return -1;
                else if (x.GetComponent<ActorController>().advantage == y.GetComponent<ActorController>().advantage)
                    return 0;
                else
                    return 1;
            }
        }
    }
}
