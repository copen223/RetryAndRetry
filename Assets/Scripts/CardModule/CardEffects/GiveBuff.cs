using Assets.Scripts.ActorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class GiveBuff:CardEffect
    {
        string buff;
        public GiveBuff(EffectTrigger trigger,string buff)
        {
            Trigger = trigger;
            this.buff = buff;
            CombatPriority = 3;
        }

        public override void DoEffect(Combat combat)
        {
            var target = isAtking ? combat.Dfder : combat.Atker;
            target.OnBuff(buff);
            base.DoEffect(combat);
        }
    }
}
