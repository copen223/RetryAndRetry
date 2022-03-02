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
        public int siblingIndex = 0;

        public override void StateStart()
        {
            base.StateStart();

            // 遮挡关系
            siblingIndex = transform.GetSiblingIndex();

            int lastSiblingIndex = Controller.Hand.GetComponent<HandController>().LastSiblingIndex;
            
            transform.SetSiblingIndex(lastSiblingIndex);


            int index = 0;
            Controller.SpriteObject.SendMessage("StartAnimation", index);
            SetEventProtect();
        }

        public override void StateUpdate()
        {
            
        }

        public override void StateExit()
        {
            transform.SetSiblingIndex(siblingIndex);

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
                if (CheckIfCanFocus())
                {
                    if (Controller.Card.type == CardUseType.Passive && Controller.Card.CardAction != null)
                        ChangeStateTo<CardSetFocus>();
                    else
                        ChangeStateTo<CardPreFocus>();
                }
                else
                {
                    Debug.Log("不在卡槽内，不能使用");
                }
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (CheckIfCanMake())
                    ChangeStateTo<CardPreMake>();
                else
                    Debug.Log("不在卡槽内，不能使用");
            }
        }

        private bool CheckIfCanMake()
        {
            // 如果打出的条件是在卡槽内？
            if (Controller.Card.Container != null)
            {
                return true;
            }
            else
                return false;

            //var actor = Controller.holder.GetComponent<ActorController>();
            //if(actor is PlayerController)
            //{
            //    PlayerController player = actor as PlayerController;
            //    if (player.ActionPoint >= 1)
            //        return true;
            //    else
            //        return false;
            //}
            //return false;
        }

        private bool CheckIfCanFocus()
        {
            if (Controller.Card.Container != null)
            {
                return true;
            }
            else
                return false;
        }
    }
}
