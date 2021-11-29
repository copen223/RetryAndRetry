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

        /// <summary>
        /// 除非类型为CardPool否则只读，外部不能修改！
        /// </summary>
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
