using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class CardPool
    {
        public PlayerController Holder;

        public List<Card> list;

        public CardPool(PlayerController holder,List<Card> cardList)
        {
            Holder = holder;
            list = cardList;
        }

        public virtual void AddCard(Card card)
        {
            list.Add(card);
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
        
    }
}
