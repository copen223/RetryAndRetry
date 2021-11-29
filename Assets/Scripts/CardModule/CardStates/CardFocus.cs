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
    class CardFocus : CardState, IPointerClickHandler
    {
        public override void StateStart()
        {
            base.StateStart();
            Controller.Card.situation = CardSituation.Focused;

            if (Mathf.Abs(Controller.SpriteObject.transform.localRotation.z - 90) > Mathf.Epsilon)
                Controller.SpriteController.SetFocusRotation();

            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        // 在换牌的时候将专注的卡状态换成了idle所以触发了exit，exit中进行了RemoveFocusTrail
        public override void StateExit()
        {
            base.StateExit();
            // 取消focus
            if (Controller.Card.IfHasTrail)
            {
                var focusObject = Controller.Card.CancleFocusTrail();
                Controller.holder.GetComponent<ActorController>().RemoveFocusTrail(focusObject);
                focusObject.SetActive(false);
                Controller.ActionController.AddNewTrailToPool(focusObject);
            }
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
                ChangeStateTo<CardUpChangePre>();
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
    }
}
