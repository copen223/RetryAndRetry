using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

namespace Assets.Scripts.BufferModule
{
    /// <summary>
    /// 挂载于一个buff物体对象上
    /// </summary>
    public class Buff : MonoBehaviour
    {
        [InspectorName("buff图片")]
        public Sprite buffUIImage;
        [InspectorName("buff名称")]
        public string BuffName;

        [Header("持续形式")]
        public BuffDurationType DurationType = BuffDurationType.Turn;
        //[Header("持续回合数")]
        public int DurationTime = 1;
        [Header("生效类型")]
        public BuffActiveType ActiveType = BuffActiveType.Sustainable;
        [Header("触发类型")]
        public BuffTriggerType TriggerType = BuffTriggerType.None;
       
        /// <summary>
        /// buff描述
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription() { return "只是一个buff"; } 



        [HideInInspector]
        public ActorController Target;
        [HideInInspector]
        public BuffController controller;

        private Card linkedCard;


        public bool IfAcitve { private set; get; }

        public void Init()
        {
                IfAcitve = true;
        }


        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public virtual void LauchBuff()
        {

        }
        /// <summary>
        /// Buff结束时调用，通常用于 永续buff将影响清除
        /// </summary>
        protected event System.Action OnBuffEndEvent;

        /// <summary>
        /// 检测条件后触发效果，用于永续且含有触发效果的buff
        /// </summary>
        /// <param name="combat"></param>
        public virtual void CheckAndTouchOffBuff(BuffTouchOffEventArgs eventArgs)
        {
            
        }


        /// <summary>
        /// 外部调用，通知该buff结束
        /// </summary>
        public void OnBuffFinished()
        {
            FinishThisBuff();
        }

        /// <summary>
        /// 作为绑定卡牌取消专注后的回调函数,同时意味着该BUFF清除
        /// </summary>
        private void CancleLinkCallback()
        {
            linkedCard.OnCancleFocusEvent -= CancleLinkCallback;
            linkedCard.OnContainerChangeEvent -= CardContainerChangeCallback;
            FinishThisBuff();

        }

        private void CardContainerChangeCallback(bool fromNull2Exist)
        {
            IfAcitve = fromNull2Exist;
        }

        /// <summary>
        /// 与卡牌绑定
        /// </summary>
        public void LinkToCard(Card card)
        {
            card.OnCancleFocusEvent += CancleLinkCallback;
            linkedCard = card;

            if (card.IfInContainer)
                IfAcitve = true;
            else
                IfAcitve = false;
        }

        /// <summary>
        /// 每回合减少1点持续时间，<=0buff结束
        /// </summary>
        /// <param name="n"></param>
        public bool ReduceTime(int n)
        {
            if (DurationType != BuffDurationType.Turn)
                return false;

            DurationTime -= n;
            if(DurationTime <= 0)
            {
                FinishThisBuff();
                return true;
            }

            return false;
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

    /// <summary>
    /// 该buff触发的时点
    /// </summary>
    public enum BuffTriggerType
    {
        None,
        /// <summary>
        /// 当造成伤害
        /// </summary>
        OnDoDamage
    }

    /// <summary>
    /// buff触发信息
    /// </summary>
    public class BuffTouchOffEventArgs
    {
        /// <summary>
        /// 触发者
        /// </summary>
        public ActorController actor;
        /// <summary>
        /// 使用什么卡牌触发
        /// </summary>
        public Card usedCard;
        /// <summary>
        /// 在进行什么combat时触发了
        /// </summary>
        public Combat combat;
    }
}
