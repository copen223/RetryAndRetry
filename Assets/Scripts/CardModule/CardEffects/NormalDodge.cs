using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class NormalDodge:CardEffect
    {
        public NormalDodge()
        {
            CombatPriority = 2;
        }
        public override void DoEffect(Combat combat)
        {
            for(int i =0;i<combat.CombatEffects.Count;i++)
            {
                var effect = combat.CombatEffects[i];

                if (effect is NormalDamage)
                {
                    if (effect.isAtking && !isAtking)
                    {
                        // 攻击方的攻击效果 防守方的该效果
                        combat.CombatEffects.Remove(effect);
                        i--;

                        NormalDamage damage = effect as NormalDamage;
                        combat.Dfder.OnDodge(new DamageData(damage.damage,-combat.Dfder.transform.position + combat.Atker.transform.position));
                    }
                    else if (!effect.isAtking && isAtking)
                    {
                        combat.CombatEffects.Remove(effect);
                        i--;
                    }
                }
            }

            base.DoEffect(combat);  //  附加effect进行
        }
    }
}
