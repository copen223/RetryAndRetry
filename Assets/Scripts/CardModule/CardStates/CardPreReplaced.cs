using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardPreReplaced : CardState
    {
        public override void StateStart()
        {
            Controller.SpriteObject.SendMessage("StartAnimation", 6);
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
            ChangeStateTo<CardAfterReplaced>();
        }
    }
}
