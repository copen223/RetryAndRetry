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

            // 为玩家增加1点行动点数
            AddActionPointForPlayer();

            if (Controller.SpriteObject.transform.localRotation.z!=90)
                Controller.SpriteController.SetFocusRotation();

            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

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
            }
        }

        public void AddActionPointForPlayer()
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
