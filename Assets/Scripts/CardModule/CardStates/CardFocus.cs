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
            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        public override void StateExit()
        {
            

        }

        public void OnPointerClick(PointerEventData eventData)
        {
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
