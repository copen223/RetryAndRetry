using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BufferModule
{
    /// <summary>
    /// 挂载于一个buff物体对象上
    /// </summary>
    public class Buff : MonoBehaviour
    {
        [Header("持续形式")]
        public BuffDurationType DurationType = BuffDurationType.Turn;
        [Header("持续回合数")]
        public int DurationTime = 1;
        [Header("生效类型")]
        public BuffActiveType ActiveType = BuffActiveType.Sustainable;

        [HideInInspector]
        public ActorController Target;
        [HideInInspector]
        public BuffController controller;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public virtual void LauchBuff()
        {

        }
        /// <summary>
        /// Buff结束时调用，将影响清除
        /// </summary>
        protected event System.Action OnBuffEndEvent;

        /// <summary>
        /// 外部调用，通知该buff结束
        /// </summary>
        public void OnBuffFinished()
        {
            FinishThisBuff();
        }
        /// <summary>
        /// 每回合减少1点持续时间，<=0buff结束
        /// </summary>
        /// <param name="n"></param>
        public void ReduceTime(int n)
        {
            DurationTime -= n;
            if(DurationTime <= 0)
            {
                FinishThisBuff();
            }
        }

        private void FinishThisBuff()
        {
            OnBuffEndEvent?.Invoke();
            controller.RemoveBuff(this);
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Buff的持续形式
    /// </summary>
    public enum BuffDurationType
    {
        Turn, // 在一定回合数后消失
        Focus,   // 专注结束前存在
        Battle   // 在战斗结束前存在
    }

    /// <summary>
    /// Buff的生效类型,决定该buff合适生效
    /// </summary>
    public enum BuffActiveType
    {
        Sustainable, // 永续
        Turn         // 每回合生效一次
    }
}
