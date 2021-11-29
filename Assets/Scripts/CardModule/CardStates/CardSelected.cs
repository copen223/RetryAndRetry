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
                if (Controller.Card.type == CardUseType.Passive)
                    ChangeStateTo<CardSetFocus>();
                else
                    ChangeStateTo<CardPreFocus>();
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (CheckIfCanMake())
                    ChangeStateTo<CardPreMake>();
                else
                    Debug.Log("没有足够的行动点数");
            }
        }

        private bool CheckIfCanMake()
        {
            var actor = Controller.holder.GetComponent<ActorController>();
            if(actor is PlayerController)
            {
                PlayerController player = actor as PlayerController;
                if (player.ActionPoint >= 1)
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
