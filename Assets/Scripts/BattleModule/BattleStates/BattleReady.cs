using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.CardModule;

namespace Assets.Scripts.BattleModule.BattleStates
{
    // 战斗准备阶段
    public class BattleReady:BattleState
    {
        public override void StateStart()
        {
            CardBuilder cardBuilder = new CardBuilder();

            foreach(var actor in Manager.ActorList)
            {
                if(actor.GetComponent<PlayerController>() != null)
                {
                    var player = actor.GetComponent<PlayerController>();
                    List<Card> cards = cardBuilder.CreatCardsByDeckInfo(player.DeckInfo);
                    player.deck = new Deck(player, cards);
                }
            }

            ChangeStateTo<BattleStart>();
        }

        private void AddActorIntoBattle(GameObject gb)
        {
            Manager.ActorList.Add(gb);
        }
    }
}
