using System.Collections.Generic;
using ActorModule.Core;
using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "恢复效果", menuName = "MyInfo/效果/恢复")]
    public class NormalRecover : CardEffect
    {
        public float RecoverValue;
        public RecoverType RecoverValueType;
        public enum RecoverType
        {
            ActionPoint
        }

        Combat combat;

        public override void DoEffect(Combat combat)
        {
            this.combat = combat;

            var user =  Card.User as PlayerController;
            switch (RecoverValueType)
            {
                case RecoverType.ActionPoint: user.ActionPoint += Mathf.RoundToInt(RecoverValue); break;
            }

          
            base.DoEffect(combat);  //  附加effect进行
        }

        Dictionary<RecoverType, string> recoverValueName = new Dictionary<RecoverType, string>
        {
            {RecoverType.ActionPoint,"行动点数" }
        };


        public override CardEffect Clone()
        {
            NormalRecover effect = CreateInstance<NormalRecover>();
            effect.RecoverValue = RecoverValue;
            effect.CombatPriority = 2;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        public override string GetDescriptionText()
        {
            return "恢复<color=blue>" + RecoverValue + "</color>点"+ recoverValueName[RecoverValueType] + ""+ GetAdditionalDescriptionText();
        }
    }
}
