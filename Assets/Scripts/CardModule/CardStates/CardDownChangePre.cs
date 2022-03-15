using System;
using ActorModule.Core;
using CardModule.CardPool;
using CardModule.Controllers;
using UI.ActionTip;

namespace CardModule.CardStates
{
    public class CardDownChangePre : CardState
    {
        CardSelectionWindowController selectionWindow;
        PlayerController player;

        public event Action OnExitFocusEvent;

        private ActionTipsUI actionTipsUI = null;
        
        public override void StateStart()
        {
            base.StateStart();
            
            actionTipsUI.SetActionTip(ActionTipType.Right,"取消",true);
            
            // 呼出选择界面
            selectionWindow = Controller.SelectionWindow.GetComponent<CardSelectionWindowController>();
            player = Controller.holder.GetComponent<PlayerController>();
            var upChangeDeck = player.upChangeDeck;
            selectionWindow.ShowCardChangeSelectionWindow(upChangeDeck.GetWuXingChangeCardList(Controller.Card, WuxingChangeType.NiKe, player.ActionPoint), OnFinishSelectCardCallBack, player.gameObject,Controller.Card);

            selectionWindow.CancleUpChangeEvent += OnCancleUpChangeCallBack;

            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, true);  // 冻结卡牌交互
        }

        /// <summary>
        /// 选择界面按下右键,取消上转，监听selectionWindow.CancleUpChangeEvent
        /// </summary>
        public void OnCancleUpChangeCallBack()
        {
            ChangeStateTo<CardFocus>();

        }
        /// <summary>
        /// 确定选择对象，进行上转，监听CardInWindowController.OnCardDoSelectedEvent
        /// </summary>
        /// <param name="selectedCard"></param>
        public void OnFinishSelectCardCallBack(Card selectedCard)
        {
            // 数据层变动
            player.ActionPoint -= (selectedCard.cardLevel - Controller.Card.cardLevel);
            player.upChangeDeck.TranslateCardTo(selectedCard, player.hand, player.hand.GetIndex(Controller.Card));
            

            // 显示层变动
            OnExitFocusEvent?.Invoke();
            ChangeStateTo<CardIdle>();
            Controller.Hand.GetComponent<HandController>().ResetHandCards();
        }

        public override void StateExit()
        {
            base.StateExit();
            
            actionTipsUI.SetAllActionTipsActive(false);
            
            selectionWindow.CancleUpChangeEvent -= OnCancleUpChangeCallBack;
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);
            selectionWindow.EndWindowShow(OnFinishSelectCardCallBack);
        }
    }
}
