using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardPreMake : CardState
    {
        List<Vector3> points = new List<Vector3>();
        int lineId;
        public override void StateStart()
        {
            transform.parent.SendMessage("OnCardMake",gameObject);
            Vector3 holderPos = GetComponent<CardController>().holder.transform.position;
            holderPos = new Vector3(holderPos.x, holderPos.y, 1);
            points.Add(holderPos);
            points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            lineId = -1;
        }

        public override void StateUpdate()
        {
            // 画线
            lineId = LineDrawer.instance.DrawLine(points, 0, lineId);
            points[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            points[1] = new Vector3(points[1].x, points[1].y, 1);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ChangeStateTo<CardDiscard>();
            }

            // 退出状态
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

                if (ifMouseOver)
                    ChangeStateTo<CardSelected>();
                else ChangeStateTo<CardIdle>();
            }
        }

        public override void StateExit()
        {
            LineDrawer.instance.FinishDrawing(lineId);
            transform.parent.SendMessage("OnCardMakingOver", gameObject);
        }


    }
}
