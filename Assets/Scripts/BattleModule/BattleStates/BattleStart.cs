using System.Collections.Generic;
using ActorModule.Core;
using UnityEngine;

namespace BattleModule.BattleStates
{
    class BattleStart : BattleState
    {
        public override void StateStart()
        {
            // 排列对象
            Manager.ActorQueue = new List<ActorController>(Manager.ActorList.ToArray());
            Manager.ActorQueue.Sort(new ActorSortByAdvantage());

            Manager.InvokeActorQueueChangeEventByState();

            Manager.CurActorIndex = 0;
            // 洗牌
            foreach(var actor in Manager.ActorList)
            {
                if(actor.GetComponent<PlayerController>() != null)
                {
                    actor.GetComponent<PlayerController>().deck.Shuffle();
                }
                if(actor.GetComponent<ActorController>()!=null)
                {
                    actor.GetComponent<ActorController>().InitPosition();
                }
            }
            
            // 切换至战斗开始
            ChangeStateTo<BattleTurnStart>();
        }

        public override void StateUpdate()
        {
            
        }

        // 排序方法
        private class ActorSortByAdvantage : IComparer<ActorController>
        {
            public int Compare(ActorController x, ActorController y)
            {
                if (x.advantage > y.advantage)
                    return -1;
                else if (x.advantage == y.advantage)
                    return 0;
                else
                    return 1;
            }
        }
    }
}
