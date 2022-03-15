using System;
using System.Collections.Generic;
using ActorModule.Core;

namespace CardModule.CardPool
{
    public class DiscardPool:CardPool
    {
        public DiscardPool(PlayerController holder, List<Card> cardList) : base(holder,cardList) { }

        public override void TranslateCardTo(Card card, CardPool pool)
        {
            if(pool is Deck && card.upChangeType == CardUpChangeType.UpChange) // 返回卡组的上转卡回到上转卡组
            {
                base.TranslateCardTo(card, Holder.upChangeDeck);
            }
            else
                base.TranslateCardTo(card, pool);
        }

        /// <summary>
        /// 随机将一张卡牌以push方式返还手卡
        /// </summary>
        /// <param name="hand"></param>
        public void RandomReturnCardToHand(CardPool hand)
        {
            Random random = new Random();
            int cardIndex = random.Next(0, list.Count - 1);

            Card card = list[cardIndex];
            list.Remove(card);

            hand.PushCard(card);
        }
        
        public override void AddCard(Card card)
        {
            base.AddCard(card);
            card.Situation = CardSituation.Idle;    // 弃掉的卡牌状态重置
        }

        public void TranslateAllCardTo(CardPool pool)
        {
            int count = list.Count;
            for(int i=0;i<count;i++)
            {
                TranslateCardTo(list[0], pool);
            }
        }
    }
}
