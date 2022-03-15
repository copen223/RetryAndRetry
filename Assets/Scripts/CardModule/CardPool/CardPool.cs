using System.Collections.Generic;
using ActorModule.Core;

namespace CardModule.CardPool
{
    public class CardPool
    {
        public readonly PlayerController Holder;

        /// <summary>
        /// 除非类型为CardPool否则只读，外部不能修改！
        /// </summary>
        public List<Card> list;

        public CardPool(PlayerController holder,List<Card> cardList)
        {
            Holder = holder;
            list = cardList;
            foreach(var card in cardList)
            {
                card.User = holder;
            }
        }

        public virtual void AddCard(Card card)
        {
            if (card.ifDisapear)
                return;
            list.Add(card);
            card.User = Holder; // 更新使用者
        }

        /// <summary>
        /// 压入卡牌
        /// </summary>
        /// <param name="card"></param>
        public virtual void PushCard(Card card)
        {
            if (card.ifDisapear)
                return;
            list.Insert(0, card);
            card.User = Holder;
        }

        public virtual void RemoveCard(Card card)
        {
            list.Remove(card);
        }

        public void ReverseCardPool()
        {
            list.Reverse();
        }

        public virtual void TranslateCardTo(Card card, CardPool pool)
        {
            RemoveCard(card);
            pool.AddCard(card);
        }
        /// <summary>
        /// 获取卡牌的index，没有返回999
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int GetIndex(Card card)
        {
            for(int i =0;i<list.Count;i++)
            {
                if(list[i] == card)
                {
                    return i;
                }
            }
            return 999;
        }
        
    }
}
