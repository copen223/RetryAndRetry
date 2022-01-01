using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BufferModule.BuffTriggers
{
    [CreateAssetMenu(fileName = "Trigger", menuName = "MyInfo/Buff触发条件/打出卡牌")]
    public class OnDoMakeCard : BuffTriggerCheck
    {
        public string CardName;

        public override bool CheckIfCanTouchOff(BuffTouchOffEventArgs eventArgs)
        {
            if (eventArgs.usedCard.name == CardName)
                return true;
            else
                return false;
        }
    }
}
