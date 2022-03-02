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
            base.StateStart();

            int index = 4;
            Controller.SpriteObject.GetComponent<CardViewController>().StartAnimation(index); 
            Controller.Hand.GetComponent<HandController>().OnCardDiscard(gameObject); 
        }

        public override void StateUpdate()
        {
            
        }

        protected override void OnAnimationDo(bool isStart)
        {
            base.OnAnimationDo(isStart);
            if(!isStart)
            {
                if (Controller.currentState != this)
                    return;
                ChangeStateTo<CardInactive>();
            }
        }

        public override void StateExit()
        {
            base.StateExit();
        }
    }
}
