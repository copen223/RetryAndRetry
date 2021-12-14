using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CardModule.CardActions;
using UnityEngine;

namespace Assets.Scripts.CardModule
{
    public abstract class CardEffect: ScriptableObject
    {
        public Card Card;

        [HideInInspector]
        public bool isAtking;           // 用于判断该效果是否为攻击方效果
        public EffectTrigger Trigger;   // 效果触发条件
       // public EffectTargetType TargetType;     // 效果发动类型
        [HideInInspector]
        public int CombatPriority;              // Combat触发时的优先级
        public EffectDoPriority DoPriority;     // 作用优先级层级
        [HideInInspector]
        public bool IfInactive = false;         // 该效果是否生效


        //public virtual void DoEffect(ActorController user, List<ActorController> targets) { }   // 针对目标的效果
        /// <summary>
        /// 针对combat的效果，在进行combat时依次触发
        /// </summary>
        /// <param name="combat"></param>
        public virtual void DoEffect(Combat combat) { foreach (var effect in AdditionalEffects_List) { effect.isAtking = isAtking; effect.DoEffect(combat); } } // 针对combat的效果

        public List<CardEffect> AdditionalEffects_List = new List<CardEffect>();    // 附加效果列表
        public void AddAdditionalEffect(CardEffect effect)
        {
            AdditionalEffects_List.Add(effect);
        }

        /// <summary>
        /// 在克隆自己时同时附带效果也要进行克隆
        /// </summary>
        /// <returns></returns>
        protected List<CardEffect> CloneAdditionalEffects()
        {
            List<CardEffect> cardEffects = new List<CardEffect>();
            foreach(var effect in AdditionalEffects_List)
            {
                cardEffects.Add(effect.Clone() as CardEffect);
            }
            return cardEffects;
        }

        /// <summary>
        /// 在创建卡牌时读取的cardinfo2中
        /// 挂载的effect如果直接赋值会造成
        /// 同一cardinfo2的多个实例使用同一个效果
        /// 因此创建新卡牌时，效果应该用clone进行处理
        /// </summary>
        /// <returns></returns>
        public abstract CardEffect Clone();

        public abstract string GetDescriptionText();

        /// <summary>
        /// 重写getdescriptiontext时要加上附加效果文本
        /// </summary>
        /// <returns></returns>
        public string GetAdditionalDescriptionText()
        {
            string text = "";
            foreach(var effect in AdditionalEffects_List)
            {
                text += "，";
                text += effect.GetDescriptionText();
            }
            return text;
        }
    }

    public enum EffectTrigger
    {
        None,
        OnMake,
        OnFocus,
        OnCombatAtk,
        OnCombatDfd,
        /// <summary>
        /// 对对方造成伤害时触发
        /// </summary>
        OnDoDamage,
        OnDiscard
    }

    public enum EffectTargetType
    {
        ChooseTarget,
        ChooseDirection,
        ChoosePos,
        Attachment
    }

    public enum EffectDoPriority
    {
        First,
        Second
    }
}
