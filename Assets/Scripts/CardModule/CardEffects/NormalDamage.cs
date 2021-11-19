using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class NormalDamage:CardEffect
    {
        public float damage;

        public NormalDamage(float _damage, EffectTrigger trigger)
        {
            damage = _damage;
            Trigger = trigger;
            CombatPriority = 1;
        }

        //public override void DoEffect(ActorController user, List<ActorController> targets)
        //{
        //    foreach(var target in targets)
        //    {
        //        var dir = (user.transform.position - target.transform.position);
        //        target.GetComponent<ActorController>().OnBehit(new DamageData(damage,dir));
        //    }
        //}

        public override void DoEffect(Combat combat)
        {
            var dir = isAtking ? combat.Atker.transform.position - combat.Dfder.transform.position : combat.Dfder.transform.position - combat.Atker.transform.position;
            if (isAtking)
            {
                combat.Dfder.OnBehit(new DamageData(damage, dir));
                var target = combat.Dfder;
                var finder = target.GetComponent<PathFinderComponent>();
                var path = finder.SearchAndGetPathByEnforcedMove(target.transform.position, -1 * dir, 2,false);
                var move = target.GetComponent<ActorMoveComponent>();
                // move.StartForceMoveByPathList(path);
            }
            else
            {
                combat.Atker.OnBehit(new DamageData(damage, dir));
            }

            base.DoEffect(combat);
        }
    }
}
