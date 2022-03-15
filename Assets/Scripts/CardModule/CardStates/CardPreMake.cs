using System.Collections.Generic;
using ActorModule.Core;
using CardModule.CardEffects;
using CardModule.Controllers;
using UI;
using UI.ActionTip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.CardStates
{
    class CardPreMake : CardState
    {
        // 控制参量
        bool IsActiveCard;


        List<Vector3> points = new List<Vector3>();
        int lineId;

        // 缓存
        List<CardEffect> WaitToDoEffects = new List<CardEffect>();  // 打出触发的效果列表
        List<CardEffect> FirstDoEffects = new List<CardEffect>();   // 第一列处理效果，往往是指向型
        List<CardEffect> SecondDoEffects = new List<CardEffect>();  // 第二列处理效果，往往是自动型
        int effectIndex;// 待处理效果索引
        bool isActing;  // 效果选定/发动/处理中
        bool isFirst;   // 正在处理第一队列

        private ActionTipsUI actionTipsUI = null;

        private void Start()
        {
            actionTipsUI = UIManager.instance.UI_ActionTips;
        }
        

        public override void StateStart()
        {
            //---------------初始化--------------
            effectIndex = 0;
            isActing = false;
            isFirst = true;
            //-----------------------------------
            
            actionTipsUI.SetActionTip(ActionTipType.Left,"确定",true);
            actionTipsUI.SetActionTip(ActionTipType.Right,"取消",true);


            base.StateStart();
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject,true);  //  告诉中央我在打出，其他的互动停止。

            Card card = Controller.Card;

            //----------阶段1，判断卡牌类型----------//
            if (card.type == CardUseType.Passive)
                IsActiveCard = false;
            else
                IsActiveCard = true;

            if(IsActiveCard)
            {
                Controller.ActionController.StartAction(card.CardAction);
                Controller.ActionController.OnActionOverEvent += OnCardActionOver;
                Controller.ActionController.OnActionCancleEvent += OnCancleMake;
            }

            if (Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
            {
                if (IsActiveCard)
                {
                    int surplusAP = player.ActionPoint - Controller.Card.cardLevel;
                    UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.red);
                }
                else
                {
                    int surplusAP = player.ActionPoint + Controller.Card.cardLevel;
                    surplusAP = surplusAP > player.ActionPoint_Max ? player.ActionPoint_Max : surplusAP;

                    UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.green);
                }
            }
        }

        bool canAction;
        int actionIndex;
        public void ActionOver()
        {
            canAction = true;
        }


        public override void StateUpdate()
        {
            // 阴牌 打出时返还点数
            if (!IsActiveCard)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // ReduceActionPoint();
                    ChangeStateTo<CardDiscard>();
                    AddActionPointByLevel();
                }
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
            Controller.ActionController.OnActionOverEvent -= OnCardActionOver;
            Controller.ActionController.OnActionCancleEvent -= OnCancleMake;

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

        public override void StateExit()
        {
            base.StateExit();
            
            if (Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
            {
                int surplusAP = player.ActionPoint;
                UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.black);
            }
            
            actionTipsUI.SetAllActionTipsActive(false);
            
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);  //  告诉中央我打出了，其他的互动可进行。
        }

        // 攻击选择结束时的操作
        public void OnCardActionOver()
        {
            Controller.ActionController.OnActionOverEvent -= OnCardActionOver;
            Controller.ActionController.OnActionCancleEvent -= OnCancleMake;

            // 如果在槽内就能打出的话
            ReduceActionPointByLevel();

            ChangeStateTo<CardDiscard>();
        }

        // 正在进行的action结束时调用该函数 攻击选择取消
        public void OnEffectActionOver()
        {
            isActing = false;
            effectIndex += 1;
        }

        private void ReduceActionPointByLevel()
        {
            if(Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.ActionPoint -= Controller.Card.cardLevel;
            }
        }

        private void AddActionPointByLevel()
        {
            if (Controller.holder.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.ActionPoint += Controller.Card.cardLevel;
            }
        }

    }
}
