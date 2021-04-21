using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class NomalDamage:CardEffect
    {
        float damage;

        public NomalDamage(float _damage, EffectTrigger trigger)
        {
            damage = _damage;
            Trigger = trigger;
        }

        public override void DoEffect(ActorController user, List<ActorController> targets)
        {
            foreach(var target in targets)
            {
                target.GetComponent<ActorController>().OnBehit(damage);
            }
        }

        public override void DoEffect(Combat combat)
        {
            if (isAtking)
                combat.Dfder.OnBehit(damage);
            else
                combat.Atker.OnBehit(damage);
        }
    }
}
