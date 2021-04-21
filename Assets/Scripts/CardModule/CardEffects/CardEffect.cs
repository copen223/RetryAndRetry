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
        public EffectTargetType TargetType;     // 效果发动类型
        public int CombatPriority;              // Combat触发时的效果
        public EffectDoPriority DoPriority;
        public virtual void DoEffect(ActorController user, List<ActorController> targets) { }
        public virtual void DoEffect(Combat combat) { } // 针对combat的效果
    }

    public enum EffectTrigger
    {
        OnMake,
        OnFocus,
        OnCombatAtk,
        OnCombatDfd
    }

    public enum EffectTargetType
    {
        ChooseTarget,
        ChooseDirection,
        ChoosePos
    }

    public enum EffectDoPriority
    {
        First,
        Second
    }
}
