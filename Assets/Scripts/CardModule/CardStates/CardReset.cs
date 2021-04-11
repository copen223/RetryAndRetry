using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardReset: CardState
    {
        public override void StateStart()
        {
            base.StateStart();
            Controller.SpriteObject.SendMessage("OnReset");
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        private void OnDraw()
        {
            if (Controller.currentState != this) return;
           // Controller.SpriteObject.SendMessage("StartAnimation", 8);
        }

        private void OnAniamtionOver()
        {
            if (Controller.currentState != this) return;
        }
    }
}
