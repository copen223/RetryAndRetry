using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardDiscard : CardState
    {
        public override void StateStart()
        {
            int index = 4;
            Controller.SpriteObject.SendMessage("StartAnimation", index);
            transform.parent.SendMessage("OnCardDiscard", gameObject);
        }

        public override void StateUpdate()
        {
            
        }

        public void OnAnimationOver()
        {
            if (Controller.currentState != this)
                return;
            ChangeStateTo<CardInactive>();
        }

        public override void StateExit()
        {
        }
    }
}
