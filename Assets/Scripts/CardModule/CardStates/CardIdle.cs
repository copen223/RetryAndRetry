using ActorModule.Core;
using CardModule.Controllers;
using UnityEngine.EventSystems;

namespace CardModule.CardStates
{
    class CardIdle: CardState,IPointerEnterHandler
    {
        public override void StateStart()
        {
            base.StateStart();

            if (Controller.Card != null)
            {
                if (Controller.Card.IfHasTrail)
                {
                    var focusObject = Controller.Card.CancleFocusTrail();
                    Controller.holder.GetComponent<ActorController>().RemoveFocusTrail(focusObject);
                    focusObject.SetActive(false);
                }
            }

            Controller.Card.Situation = CardSituation.Idle;
            int index = 1;
            Controller.SpriteObject.GetComponent<CardViewController>().StartAnimation(index);
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Controller.currentState != this)
                return;
            ChangeStateTo<CardSelected>();
        }
    }
}
