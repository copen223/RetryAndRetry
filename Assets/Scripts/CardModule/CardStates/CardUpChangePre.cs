using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardUpChangePre : CardState
    {
        CardSelectionWindowController selectionWindow;
        PlayerController player;
        public override void StateStart()
        {
            base.StateStart();
            // 呼出选择界面
            selectionWindow = Controller.SelectionWindow.GetComponent<CardSelectionWindowController>();
            player = Controller.holder.GetComponent<PlayerController>();
            var upChangeDeck = player.upChangeDeck;
            selectionWindow.ShowCardSelectionWindow(upChangeDeck.GetUpChangeCardList(Controller.Card),OnFinishSelectCardCallBack, player.gameObject);
            
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
            player.upChangeDeck.TranslateCardTo(selectedCard, player.hand, player.hand.GetIndex(Controller.Card));

            player.ActionPoint -= selectedCard.cardLevel;

            // 显示层变动
            ChangeStateTo<CardIdle>();
            Controller.Hand.GetComponent<HandController>().ResetHandCards();
        }

        public override void StateExit()
        {
            base.StateExit();
            selectionWindow.CancleUpChangeEvent -= OnCancleUpChangeCallBack;
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);
            selectionWindow.EndWindowShow(OnFinishSelectCardCallBack);
        }
    }
}
