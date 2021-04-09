using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardAfterReplaced : CardState
    {
        public override void StateStart()
        {
            Controller.Card = Controller.ToBeReplacedCard;                  // 更新卡牌(这里应该改用发信息)
            Controller.SpriteObject.SendMessage("StartAnimation", 7);       // 旋转动画后段
            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        public override void StateExit()
        {
            

        }

        public void OnAnimationOver()
        {
            if (Controller.currentState!= this)
                return;
            ChangeStateTo<CardIdle>();
        }
    }
}
