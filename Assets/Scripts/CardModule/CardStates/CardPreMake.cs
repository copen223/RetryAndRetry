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
        // 控制参量
        bool IsActiveCard;


        List<Vector3> points = new List<Vector3>();
        int lineId;
        public override void StateStart()
        {
            base.StateStart();
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject,true);  //  告诉中央我在打出，其他的互动停止。

            Card card = Controller.Card;

            //----------阶段1，判断卡牌类型----------//
            if (card.type == CardType.Passive)
                IsActiveCard = false;
            else
                IsActiveCard = true;



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
        public void OnCancleMake()
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
        public void OnFinishPre()
        { }

        public override void StateExit()
        {
            base.StateExit();

            LineDrawer.instance.FinishDrawing(lineId);
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);  //  告诉中央我打出了，其他的互动可进行。
        }


    }
}
