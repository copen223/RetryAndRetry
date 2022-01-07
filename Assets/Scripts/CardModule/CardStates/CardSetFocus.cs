using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    // 设置专注位置的状态
    class CardSetFocus : CardState
    {
        public override void StateStart()
        {
            base.StateStart();
            SetEventProtect();

            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, true);  //  告诉中央我在打出，其他的互动停止。
            
            // 在actioncontroller中执行设置专注位置的行动
            Controller.ActionController.StartAction(Controller.Card.CardAction);
            Controller.ActionController.OnActionOverEvent += OnActionOver;
        }

        public override void StateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                bool ifMouseOver = false;
                List<RaycastResult> results = new List<RaycastResult>();
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                EventSystem.current.RaycastAll(eventData, results);
                if (results.Count > 0)
                {
                    foreach (var result in results)
                    {
                        var gb = ExecuteEvents.GetEventHandler<IEventSystemHandler>(result.gameObject);
                        if (gb == gameObject) ifMouseOver = true;
                    }
                }
                // 取消订阅
                Controller.ActionController.OnActionOverEvent -= OnActionOver;

                if (ifMouseOver)
                    ChangeStateTo<CardSelected>();
                else ChangeStateTo<CardIdle>();
            }
        }

        public override void StateExit()
        {
            base.StateExit();
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);  //  告诉中央我在打出，其他的互动停止。
        }

        public void OnActionOver()
        {
            Controller.ActionController.OnActionOverEvent -= OnActionOver;
            ChangeStateTo<CardPreFocus>();
        }
    }
}
