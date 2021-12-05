using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    // 卡牌专注时的状态，点击右键可以取消还原
    class CardFocus : CardState, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        private bool ifFinishListenEvent = false;

        public override void StateStart()
        {
            base.StateStart();
            if (!ifFinishListenEvent)
            {
                Controller.GetComponent<CardUpChangePre>().OnExitFocusEvent += ExitFocusCallBack;
                Controller.GetComponent<CardDownChangePre>().OnExitFocusEvent += ExitFocusCallBack;
                ifFinishListenEvent = true;
            }

            Controller.Card.situation = CardSituation.Focused;

            if (Mathf.Abs(Controller.SpriteObject.transform.localRotation.eulerAngles.z - 90) > Mathf.Epsilon)
                Controller.SpriteController.SetFocusRotation();

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
                    Controller.ActionController.AddNewTrailToPool(focusObject);
                }
            }
        }


        // 在换牌的时候将专注的卡状态换成了idle所以触发了exit，exit中进行了RemoveFocusTrail
        public override void StateExit()
        {
            base.StateExit();

            Controller.Card.ShowFocusTrail(false);

            if (ifUpChange)
            {
                ifUpChange = false;
                return;
            }
            if (Controller.Card != null)
            {
                if (Controller.Card.situation == Assets.Scripts.CardModule.CardSituation.Focused)
                    return;
                // 取消focus
                if (Controller.Card.IfHasTrail)
                {
                    var focusObject = Controller.Card.CancleFocusTrail();
                    Controller.holder.GetComponent<ActorController>().RemoveFocusTrail(focusObject);
                    focusObject.SetActive(false);
                    Controller.ActionController.AddNewTrailToPool(focusObject);
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
            Controller.Card.ShowFocusTrail(false);
        }
    }
}
