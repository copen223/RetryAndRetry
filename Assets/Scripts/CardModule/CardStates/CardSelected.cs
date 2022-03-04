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

            // 显示消耗
            CheckIfCanMake(false);

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
            if (Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
            {
                int surplusAP = player.ActionPoint;
                UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.black);
            }

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
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (CheckIfCanMake(true))
                    ChangeStateTo<CardPreMake>();
                   
            }
        }

        private bool CheckIfCanMake(bool ifPrint)
        {
            // 如果打出的条件是在卡槽内？
            if (Controller.Card.Container != null)
            {
                if(Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    if (Controller.Card.type == CardUseType.Passive)
                    {
                        int surplusAP = player.ActionPoint + Controller.Card.cardLevel;
                        surplusAP = surplusAP > player.ActionPoint_Max ? player.ActionPoint_Max : surplusAP;

                        UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.green);

                        return true;
                    }
                    if (player.ActionPoint >= Controller.Card.cardLevel)
                    {
                        int surplusAP = player.ActionPoint - Controller.Card.cardLevel;
                        UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.red);

                        return true;
                    }
                    else if (ifPrint)
                    {
                        print("点数不足");
                        UIManager.instance.CreatFloatUIAt(player.gameObject, Vector2.zero, 2f, Color.black, "行动点数不足！");
                    }
                }
            }
            else if(ifPrint)
            {
                print("不在卡槽中");
                UIManager.instance.CreatFloatUIAt(Controller.Card.User.gameObject, Vector2.zero, 2f, Color.black, "不在卡槽中，不能打出");
            }
            
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
            {
                print("不在卡槽中，不能专注");
                UIManager.instance.CreatFloatUIAt(Controller.Card.User.gameObject, Vector2.zero, 2f, Color.black, "不在卡槽中，不能专注");
                return false;
            }
        }
    }
}
