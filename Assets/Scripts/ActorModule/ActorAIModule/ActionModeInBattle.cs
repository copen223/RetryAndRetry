using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorModule.AI
{
    // 行动模式父类，其他行动模式都派生自该类
    public class ActionModeInBattle : MonoBehaviour
    {
        public GameObject Actor { get { return transform.parent.gameObject; } }

        public enum ActionIntention
        {
            None,
            AttackUp,
            AttackDown
        }

        private ActionIntention nextActionIntention;

        /// <summary>
        /// 控制器在AI的Action阶段调用
        /// </summary>
        public virtual void StartDoBattleAI()
        {
            
        }

        private void ShowActionIntentionCallBack()
        {
            if(nextActionIntention == ActionIntention.AttackDown)
            {

            }
            else if(nextActionIntention == ActionIntention.AttackUp)
            {

            }
        }
    }
}
