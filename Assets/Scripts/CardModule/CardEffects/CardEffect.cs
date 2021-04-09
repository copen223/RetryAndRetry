using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class CardEffect
    {
        public EffectTrigger trigger;   // 效果触发条件
        public EffectDoType doType;     // 效果发动类型

        public virtual void DoEffect(ActorController user, List<ActorController> targets) { }

    }

    public enum EffectTrigger
    {
        OnMake,
        OnFocus
    }

    public enum EffectDoType
    {
        ChooseTarget,
        ChooseDirection,
        ChoosePos
    }
}
