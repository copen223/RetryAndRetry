using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ActorModule.AI
{
    // 行动方案父类，具体的行动方案都派生自该类
    public class ActionPlan : MonoBehaviour
    {
        // 链接
        public ActionModeInBattle ActionMode { get { return transform.parent.GetComponent<ActionModeInBattle>(); } }
        public GameObject Actor { get { return ActionMode.Actor; } }
        public GameObject AI { get { return ActionMode.gameObject; } }

        [HideInInspector]
        public bool IfPlanOver = false;

        public virtual void DoPlan()
        {

        }

        /// <summary>
        /// 设置手卡卡牌 以展示行动意图
        /// </summary>
        public virtual void SetHandCards() { }

        /// <summary>
        /// 计划执行结束时触发的事件，通常用于通知ActionMode行动完毕可以结束回合了。
        /// </summary>
        public event Action ActionPlanOverEvent;

        /// <summary>
        /// 派生的子类发布行动结束通知时应调用的方法
        /// </summary>
        protected void InvokeActionPlanOverEvent()
        {
            ActionPlanOverEvent?.Invoke();
        }
    }
}
