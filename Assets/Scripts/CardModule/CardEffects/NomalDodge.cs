using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class NomalDodge:CardEffect
    {
        public NomalDodge()
        {
            CombatPriority = 2;
        }
        public override void DoEffect(Combat combat)
        {
            for(int i =0;i<combat.CombatEffects.Count;i++)
            {
                var effect = combat.CombatEffects[i];

                if (effect is NomalDamage)
                {
                    if (effect.isAtking && !isAtking)
                    {
                        combat.CombatEffects.Remove(effect);
                        i--;
                    }
                    else if (!effect.isAtking && isAtking)
                    {
                        combat.CombatEffects.Remove(effect);
                        i--;
                    }
                }
            }
        }
    }
}
