using ActorModule.Core;
using CardModule.Controllers;
using UI;
using UI.ActionTip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.CardStates
{
    class CardSelected: CardState,IPointerExitHandler,IPointerClickHandler
    {
        public int siblingIndex = 0;
        private ActionTipsUI actionTipsUI = null;

        private void Start()
        {
            actionTipsUI = UIManager.instance.UI_ActionTips;
        }

        public override void StateStart()
        {
            base.StateStart();
            
            // 提示窗口
            actionTipsUI.SetAllActionTipsActive(false);
            actionTipsUI.SetActionTip(ActionTipType.Left,"打出",true);
            actionTipsUI.SetActionTip(ActionTipType.Right,"专注",true);

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
            
            actionTipsUI.SetAllActionTipsActive(false);

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
                        return true;
                    }
                    if (player.ActionPoint >= Controller.Card.cardLevel)
                    {
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
