using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardFocus : CardState, IPointerClickHandler
    {
        public override void StateStart()
        {
            base.StateStart();
            Controller.Card.situation = CardSituation.Focused;

            if(Controller.SpriteObject.transform.localRotation.z!=90)
                Controller.SpriteController.SetFocusRotation();

            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        public override void StateExit()
        {
            base.StateExit();

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Controller.canInteract)
                return;
            if (Controller.currentState != this)
                return;
            if (IsEventProtecting)
                return;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ChangeStateTo<CardAfterFocus>();
            }
        }
    }
}
