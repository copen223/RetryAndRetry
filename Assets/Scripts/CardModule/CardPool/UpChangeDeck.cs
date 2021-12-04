using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class UpChangeDeck : CardPool
    {
        public UpChangeDeck(PlayerController holder, List<Card> cardList) : base(holder, cardList) { }

        /// <summary>
        /// 获取该卡牌的上转卡牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public List<Card> GetUpChangeCardList(Card baseCard)
        {
            List<Card> upChangeCards = new List<Card>();
            int baseLevel = baseCard.cardLevel;
            int targetLevel = baseLevel += 1;


            foreach(var card in list)
            {
                if(card.cardLevel == targetLevel && CheckWuXing(baseCard ,card))
                {
                    upChangeCards.Add(card);
                }
            }

            return upChangeCards;
        }

        private bool CheckWuXing(Card orgin,Card end)
        {
            if (ShengDic[orgin.cardElement] == end.cardElement)
                return true;
            return false;
        }

        private Dictionary<CardElement, CardElement> ShengDic = new Dictionary<CardElement, CardElement>
        {
            { CardElement.Mu,CardElement.Huo },
            { CardElement.Huo,CardElement.Tu },
            { CardElement.Tu,CardElement.Jin },
            { CardElement.Shui,CardElement.Mu },
            { CardElement.Jin,CardElement.Shui },
        };

        /// <summary>
        /// 将池子里的card和hand index位置的卡牌进行交换，原有卡牌丢弃
        /// </summary>
        /// <param name="card"></param>
        /// <param name="hand"></param>
        /// <param name="index"></param>
        /// <param name="discard"></param>
        public void TranslateCardTo(Card card,Hand hand, int index)
        {
            RemoveCard(card);
            hand.ReplaceCardTo(index, card, Holder.discard);
        }


    }
}
