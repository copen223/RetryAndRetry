using System.Collections.Generic;
using ActorModule.Core;

namespace CardModule.CardPool
{
    public enum WuxingChangeType
    {
        Sheng,
        NiKe
    }

    public class UpChangeDeck : CardPool
    {
        public UpChangeDeck(PlayerController holder, List<Card> cardList) : base(holder, cardList) { }

        /// <summary>
        /// 获取该卡牌的上转卡牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public List<Card> GetWuXingChangeCardList(Card baseCard,WuxingChangeType type,int maxlevelChange)
        {
            List<Card> upChangeCards = new List<Card>();
            int baseLevel = baseCard.cardLevel;
            int targetLevel = -1;
            if (type == WuxingChangeType.Sheng) targetLevel = baseLevel + maxlevelChange;
            else if (type == WuxingChangeType.NiKe) targetLevel = baseLevel - maxlevelChange;

            if (targetLevel <= 0)
                targetLevel = 0;


            foreach(var card in list)
            {
                if (type == WuxingChangeType.Sheng)
                {
                    if (card.cardLevel <= targetLevel && card.cardLevel >= baseLevel && CheckWuXingSheng(baseCard, card))
                    {
                        upChangeCards.Add(card);
                    }
                }
                else if (type == WuxingChangeType.NiKe)
                {
                    if (card.cardLevel >= targetLevel && card.cardLevel <= baseLevel && CheckWuxingKe(card, baseCard))
                    {
                        upChangeCards.Add(card);
                    }
                }
            }
            return upChangeCards;
        }


        private bool CheckWuXingSheng(Card source,Card end)
        {
            if (ShengDic[source.cardElement] == end.cardElement)
                return true;
            return false;
        }
        /// <summary>
        /// 向被克关系演变
        /// </summary>
        /// <param name="source"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool CheckWuxingKe(Card source,Card end)
        {
            if (KeDic[source.cardElement] == end.cardElement)
                return true;
            else
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

        private Dictionary<CardElement, CardElement> KeDic = new Dictionary<CardElement, CardElement>
        {
            { CardElement.Mu,CardElement.Tu },
            { CardElement.Tu,CardElement.Shui },
            { CardElement.Shui,CardElement.Huo },
            { CardElement.Huo,CardElement.Jin },
            { CardElement.Jin,CardElement.Mu },
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
