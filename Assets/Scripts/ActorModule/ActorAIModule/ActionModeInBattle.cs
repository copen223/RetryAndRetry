using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorModule.AI
{
    // 行动模式父类，其他行动模式都派生自该类
    public class ActionModeInBattle : MonoBehaviour
    {
        protected List<ActionPlan> actionPlans_list;    // 行动方案列表

        public virtual void StartDoBattleAI()
        {
        
        }
    }
}
