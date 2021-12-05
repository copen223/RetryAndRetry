using Assets.Scripts.ActorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardEffects
{
    public class NormalDodge:CardEffect
    {
        Combat combat;
        public int dodgeValue;
        public NormalDodge(int dodgeValue)
        {
            this.dodgeValue = dodgeValue; 
            CombatPriority = 2;
        }
        /// <summary>
        /// combat结束后清楚数值变化
        /// </summary>
        public void ClearInfluenceCallBack()
        {
            Card.User.Ability.Dodge.AddTheAddValue(-dodgeValue);
            combat.CombatEndEvent -= ClearInfluenceCallBack;
        }

        public override void DoEffect(Combat combat)
        {
            this.combat = combat;
            // 判断是否可以进行闪避
            ActorController target = isAtking ? combat.Dfder : combat.Atker;
            ActorController user = Card.User;

            user.Ability.Dodge.AddTheAddValue(dodgeValue);

            // 结束时清楚数值改动
            combat.CombatEndEvent += ClearInfluenceCallBack;


            //for(int i =0;i<combat.CombatEffects.Count;i++)
            //{
            //    var effect = combat.CombatEffects[i];



            //    //if (effect is NormalDamage)
            //    //{
            //    //    if (effect.isAtking && !isAtking)
            //    //    {
            //    //        // 攻击方的攻击效果 防守方的该效果
            //    //        combat.CombatEffects.Remove(effect);
            //    //        i--;

            //    //        NormalDamage damage = effect as NormalDamage;
            //    //        combat.Dfder.OnDodge(new DamageData(damage.damage,-combat.Dfder.transform.position + combat.Atker.transform.position));
            //    //    }
            //    //    else if (!effect.isAtking && isAtking)
            //    //    {
            //    //        combat.CombatEffects.Remove(effect);
            //    //        i--;
            //    //    }
            //    //}
            //}

            base.DoEffect(combat);  //  附加effect进行
        }
    }
}
