using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorModule.AI
{
    // 行动模式父类，其他行动模式都派生自该类
    public class ActionModeInBattle : MonoBehaviour
    {
        public GameObject Actor { get { return transform.parent.gameObject; } }

        /// <summary>
        /// 控制器在AI的Action阶段调用
        /// </summary>
        public virtual void StartDoBattleAI()
        {
            
        }
    }
}
