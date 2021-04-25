using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardSelected: CardState,IPointerExitHandler,IPointerClickHandler
    {
        public override void StateStart()
        {
            base.StateStart();
            int index = 0;
            Controller.SpriteObject.SendMessage("StartAnimation", index);
            SetEventProtect();
        }

        public override void StateUpdate()
        {
            
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Controller.currentState != this)
                return;

            ChangeStateTo<CardIdle>(); 
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
                if (Controller.Card.type == CardType.Passive)
                    ChangeStateTo<CardSetFocus>();
                else
                    ChangeStateTo<CardPreFocus>();
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ChangeStateTo<CardPreMake>();
            }
        }
    }
}
