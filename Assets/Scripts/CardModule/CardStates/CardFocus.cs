using ActorModule.Core;
using UI;
using UI.ActionTip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.CardStates
{
    // 卡牌专注时的状态，点击右键可以取消还原
    class CardFocus : CardState, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        private bool ifFinishListenEvent = false;

        private ActionTipsUI actionTipsUI = null;
        private void Start()
        {
            actionTipsUI = UIManager.instance.UI_ActionTips;
        }

        public override void StateStart()
        {
            base.StateStart();
            if (!ifFinishListenEvent)
            {
                Controller.GetComponent<CardUpChangePre>().OnExitFocusEvent += ExitFocusCallBack;
                Controller.GetComponent<CardDownChangePre>().OnExitFocusEvent += ExitFocusCallBack;
                ifFinishListenEvent = true;
            }

            Controller.Card.Situation = CardSituation.Focused;
            Controller.OnReset();

            if(Controller.ifMouseSelectThis)
            {
                Controller.Card.User.ActiveAllFocusTrail(true);
                Controller.Card.ShowFocusTrail(true);
                Controller.SpriteController.StartAnimation(0);
            }
            else
            {
                Controller.Card.User.ActiveAllFocusTrail(false);
                Controller.Card.ShowFocusTrail(false);
                Controller.SpriteController.StartAnimation(1);
            }

            //if (Mathf.Abs(Controller.SpriteObject.transform.localRotation.eulerAngles.z - 90) > Mathf.Epsilon)
            //    Controller.SpriteController.SetFocusRotation();

            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        private bool ifUpChange = false;

        /// <summary>
        /// 升阶等选择界面时调用
        /// </summary>
        public void ExitFocusCallBack()
        {
            if (Controller.Card != null)
            {
                // 取消focus
                if (Controller.Card.IfHasTrail)
                {
                    var focusObject = Controller.Card.CancleFocusTrail();
                    Controller.holder.GetComponent<ActorController>().RemoveFocusTrail(focusObject);
                    focusObject.SetActive(false);
                }
            }
        }


        // 在换牌的时候将专注的卡状态换成了idle所以触发了exit，exit中进行了RemoveFocusTrail
        public override void StateExit()
        {
            base.StateExit();
            
            actionTipsUI.SetAllActionTipsActive(false);
            
            Controller.Card.ShowFocusTrail(false);

            if (ifUpChange)
            {
                ifUpChange = false;
                return;
            }
            if (Controller.Card != null)
            {
                if (Controller.Card.Situation == CardSituation.Focused)
                    return;
                // 取消focus
                if (Controller.Card.IfHasTrail)
                {
                    var focusObject = Controller.Card.CancleFocusTrail();
                    Controller.holder.GetComponent<ActorController>().RemoveFocusTrail(focusObject);
                    focusObject.SetActive(false);
                }
            }

            // Controller.Card.situation = CardSituation.Idle;
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
                // 禁止取消专注
                // ChangeStateTo<CardAfterFocus>();
                // 卡牌升阶
                ifUpChange = true;
                ChangeStateTo<CardUpChangePre>();
            }
            else if(eventData.button == PointerEventData.InputButton.Left)
            {
                ifUpChange = false;
                ChangeStateTo<CardDownChangePre>();
            }
        }

        private void AddActionPointForPlayer()
        {
            var actor = Controller.holder.GetComponent<ActorController>();
            if(actor is PlayerController)
            {
                var player = actor as PlayerController;
                player.ActionPoint += 1;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Controller.canInteract)
                return;
            if (Controller.currentState != this)
                return;
            if (IsEventProtecting)
                return;

            var actionTipsUI = UIManager.instance.UI_ActionTips;
            actionTipsUI.SetAllActionTipsActive(false);
            actionTipsUI.SetActionTip(ActionTipType.Left,"下转",true);
            actionTipsUI.SetActionTip(ActionTipType.Right,"上转",true);
            
            Controller.SpriteController.StartAnimation(0);

            Controller.Card.User.ActiveAllFocusTrail(true);
            Controller.Card.ShowFocusTrail(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Controller.canInteract)
                return;
            if (Controller.currentState != this)
                return;
            if (IsEventProtecting)
                return;
            
            actionTipsUI.SetAllActionTipsActive(false);
            
            Controller.SpriteController.StartAnimation(1);

            Controller.Card.User.ActiveAllFocusTrail(false);
            Controller.Card.ShowFocusTrail(false);
        }

    }
}
