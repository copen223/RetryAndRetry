using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardEffects;

namespace ActorModule.AI
{
    /// <summary>
    /// 就近攻击行动方案
    /// </summary>
    public class ProximityAttackAP : ActionPlan
    {

        private Dictionary<GameObject, float> heatLevelOfEnemies_dic = new Dictionary<GameObject, float>();  // 威胁度字典

        private Card attackCard;

        public override void DoPlan()
        {
            StartCoroutine(DoPlanCoroutine());
        }

        private IEnumerator DoPlanCoroutine()
        {
            UpdateHeatLevels();

            yield return new WaitForSeconds(3f);

            InvokeActionPlanOverEvent();
        }

        #region 威胁度更新
        /// <summary>
        /// 更新威胁度字典
        /// </summary>
        private void UpdateHeatLevels()
        {
            heatLevelOfEnemies_dic.Clear();
            foreach(var go in BattleManager.instance.ActorQueue)
            {
                heatLevelOfEnemies_dic.Add(go, UpdateHeatLevel(go));
            }
        }

        /// <summary>
        /// UpdateHeatLevels调用，计算单个对象的威胁度
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private float UpdateHeatLevel(GameObject target)
        {
            float heatLevel = 0;

            float dis = Vector2.Distance(Actor.transform.position, target.transform.position);
            float canAttack_value = CheckIfCanAttack(target) ? 10f : 0;
            float healPoint_value = GetTargetHealPointValue(target);
            

            return heatLevel;
        }

        private bool CheckIfCanAttack(GameObject target)
        {
            return false;
        }

        private float GetTargetHealPointValue(GameObject target)
        {
            float v = 0;
            var actor = target.GetComponent<ActorController>();
            float hp = 100f * actor.HealPoint/actor.HealPoint_Max;
            v += (100f - hp) * 0.1f;

            return v;
        }
        #endregion
    }
}
