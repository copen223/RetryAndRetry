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
            selectionWindow.ShowCardSelectionWindow(upChangeDeck.GetUpChangeCardList(Controller.Card),OnFinishSelectCardCallBack);
            
            selectionWindow.CancleUpChangeEvent += OnCancleUpChangeCallBack;

            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, true);  // 冻结卡牌交互
        }

        /// <summary>
        /// 选择界面按下右键,取消上转
        /// </summary>
        public void OnCancleUpChangeCallBack()
        {
            ChangeStateTo<CardFocus>();

        }
        /// <summary>
        /// 确定选择对象，进行上转
        /// </summary>
        /// <param name="selectedCard"></param>
        public void OnFinishSelectCardCallBack(Card selectedCard)
        {
            // 数据层变动
            player.upChangeDeck.TranslateCardTo(selectedCard, player.hand, player.hand.GetIndex(Controller.Card));
            // 显示层变动
            Controller.Hand.GetComponent<HandController>().ResetHandCards();
            ChangeStateTo<CardIdle>();
        }

        public override void StateExit()
        {
            base.StateExit();
            Controller.Hand.GetComponent<HandController>().OnCardMakeDo(gameObject, false);
            selectionWindow.EndWindowShow(OnFinishSelectCardCallBack);
        }
    }
}
