using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CardModule.CardActions;

namespace Assets.Scripts.CardModule
{
    public class CardEffect
    {
        public bool isAtking;           // 用于判断该效果是否为攻击方效果
        public EffectTrigger Trigger;   // 效果触发条件
       // public EffectTargetType TargetType;     // 效果发动类型
        public int CombatPriority;              // Combat触发时的优先级
        public EffectDoPriority DoPriority;     // 作用优先级层级
        //public virtual void DoEffect(ActorController user, List<ActorController> targets) { }   // 针对目标的效果
        public virtual void DoEffect(Combat combat) { foreach (var effect in AdditionalEffects_List) { effect.isAtking = isAtking; effect.DoEffect(combat); } } // 针对combat的效果

        public List<CardEffect> AdditionalEffects_List = new List<CardEffect>();    // 附加效果列表
        public void AddAdditionalEffect(CardEffect effect)
        {
            AdditionalEffects_List.Add(effect);
        }
    }

    public enum EffectTrigger
    {
        None,
        OnMake,
        OnFocus,
        OnCombatAtk,
        OnCombatDfd
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
