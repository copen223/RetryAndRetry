using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.CardStates
{
    // 取消专注时的动画表现，最后转到idle状态
    class CardAfterFocus: CardState
    {
        public override void StateStart()
        {
            base.StateStart();

            Controller.SpriteObject.SendMessage("StartAnimation", 3);
            SetEventProtect();
        }

        public override void StateUpdate()
        {
            
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        protected override void OnAnimationDo(bool isStart)
        {
            base.OnAnimationDo(isStart);
            if(!isStart)
            {
                if (Controller.currentState != this)
                    return;
                //if (IsEventProtecting)
                //    return;
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

                if (ifMouseOver)
                    ChangeStateTo<CardSelected>();
                else ChangeStateTo<CardIdle>();

            }
        }
    }
}
