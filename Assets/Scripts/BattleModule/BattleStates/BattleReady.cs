using System.Collections.Generic;
using ActorModule.Core;
using CardModule;
using CardModule.CardPool;
using UnityEngine;

namespace BattleModule.BattleStates
{
    // 战斗准备阶段 主要是用卡牌构造器从对象的卡组信息进行卡组实例的初始化
    public class BattleReady:BattleState
    {
        public override void StateStart()
        {
            CardBuilder2 cardBuilder = new CardBuilder2();

            foreach(var actor in Manager.ActorList)
            {
                if(actor.GetComponent<PlayerController>() != null)
                {
                    var player = actor.GetComponent<PlayerController>();
                    List<Card> cards = cardBuilder.CreatCardsByDeckInfo(player.DeckInfo);
                    player.deck = new Deck(player, cards);
                    List<Card> upCards = cardBuilder.CreatCardsByDeckInfo(player.UpChangeDeckInfo);
                    player.upChangeDeck = new UpChangeDeck(player, upCards);
                }
            }

            ChangeStateTo<BattleStart>();
        }

        private void AddActorIntoBattle(GameObject gb)
        {
            Manager.ActorList.Add(gb.GetComponent<ActorController>());
        }
    }
}
