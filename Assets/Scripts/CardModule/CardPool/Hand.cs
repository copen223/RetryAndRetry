using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule
{
    public class Hand : CardPool
    {
        public Hand(PlayerController holder, List<Card> cardList) : base(holder, cardList) { Containers = holder.containers; }

        public List<Container> Containers;

        public override void AddCard(Card card)
        {
            // 添加卡牌
            base.AddCard(card);
            // 对应卡槽
            if (Containers.Count >= list.Count)
                card.Container = Containers[list.Count - 1];
            else
                card.Container = null;
        }

        public override void RemoveCard(Card card)
        {
            base.RemoveCard(card);
            int i = 0;
            for(;i< Containers.Count && i<list.Count;i++)
            {
                Containers[i].Card = list[i];
            }
            for(;i<Containers.Count;i++)
            {
                Containers[i].Card = null;
            }
        }

        private void ReplaceAllCard(List<Container> containers,Deck deck,DiscardPool discard)
        {
            int i;
            for(i=0;i< containers.Count;i++)
            {
                var container = containers[i];
                if (container.Card == null)
                {
                    TranslateCardTo(deck.GetFirstCard(container.type), this);
                    container.Card = list[i];
                }
                else
                {
                    // 换牌
                    var rep = deck.GetFirstCard(containers[i].type);
                    list[i] = rep;  // 手牌已换
                    discard.AddCard(rep); // 弃牌区已更新
                    deck.RemoveCard(rep); // 卡组已更新
                }
            }   
        }
    }
}
