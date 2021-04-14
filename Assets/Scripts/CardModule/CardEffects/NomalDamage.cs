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

        public NomalDamage(float _damage)
        {
            damage = _damage;
        }

        public override void DoEffect(ActorController user, List<ActorController> targets)
        {
            foreach(var target in targets)
            {
                target.GetComponent<ActorController>().OnBehit(damage);
            }
        }
    }
}
