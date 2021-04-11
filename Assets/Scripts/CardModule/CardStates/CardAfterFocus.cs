using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardAfterFocus: CardState
    {
        public override void StateStart()
        {
            base.StateStart();

            Controller.SpriteObject.SendMessage("StartAnimation", 3);
            SetEventProtect();
            Controller.Hand.GetComponent<HandController>().HandBroadcastMessage("OnAnimationStart");
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

                Controller.Hand.GetComponent<HandController>().HandBroadcastMessage("OnAnimationOver");
            }
        }
    }
}
