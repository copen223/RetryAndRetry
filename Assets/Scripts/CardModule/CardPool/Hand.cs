﻿using System;
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

        /// <summary>
        /// 如果是丢弃手牌的卡，除了该方法以外，还要将focustrail与card、actor解绑，以及销毁focustrail！
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pool"></param>
        public override void TranslateCardTo(Card card, CardPool pool)
        {
            base.TranslateCardTo(card, pool);
        }

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
        /// <summary>
        /// 外部应用translate！
        /// </summary>
        /// <param name="card"></param>
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
        /// <summary>
        /// 将index位置的卡牌替换为card，原卡牌送去pool
        /// </summary>
        /// <param name="index"></param>
        /// <param name="card"></param>
        /// <param name="pool"></param>
        public void ReplaceCardTo(int index,Card card,CardPool pool)
        {
            var replaced = list[index];
            pool.AddCard(replaced);
            list[index] = card;
            int i = 0;
            for (; i < Containers.Count && i < list.Count; i++)
            {
                Containers[i].Card = list[i];
            }
            for (; i < Containers.Count; i++)
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
