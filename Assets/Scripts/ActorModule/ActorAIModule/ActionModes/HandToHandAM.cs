using System.Collections;
using System.Collections.Generic;
using ActorModule.ActorAIModule.ActionPlans;
using BattleModule;
using BattleModule.BattleStates;
using UnityEngine;

namespace ActorModule.ActorAIModule.ActionModes
{
    /// <summary>
    /// 肉搏模式AM，总是寻找最近的目标进行攻击
    /// </summary>
    public class HandToHandAM : ActionModeInBattle
    {
        // 攻击计划，这是默认的行动计划
        private ProximityAttackAP attackPlan { get {return transform.Find("ActionPlans").GetComponent<ProximityAttackAP>(); } }
        private ActionPlan curPlan;

        private void Start()
        {
            curPlan = SelectPlan();
            curPlan.SetHandCards();
        }

        #region 行动流程


        /// <summary>
        /// 开始行动
        /// </summary>
        public override void StartDoBattleAI()
        {
            curPlan = SelectPlan();
            curPlan.ActionPlanOverEvent += OnActionOver;
            curPlan.DoPlan();
        }

        /// <summary>
        /// 行动结束时触发的函数
        /// </summary>
        private void OnActionOver()
        {
            curPlan.ActionPlanOverEvent -= OnActionOver;

            BattleManager.instance.GetComponent<BattleTurnAction>().OnTurnEnd();
        }

        #endregion
        IEnumerator Wait(float time)
        {
            float timer = 0;
            while(timer < time)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            BattleManager.instance.GetComponent<BattleTurnAction>().OnTurnEnd();
        }

        /// <summary>
        /// 这个方法决定该模式下什么情况使用什么样的计划
        /// </summary>
        /// <returns></returns>
        ActionPlan SelectPlan()
        {
            return attackPlan;
        }

        
    }
}
