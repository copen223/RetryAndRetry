using System.Collections.Generic;
using CardModule.CardEffects;
using Resources.Infos;

namespace CardModule
{
    public class CardBuilder2
    {
        public List<Card> CreatCardsByDeckInfo(DeckInfo info)
        {
            List<Card> deckCards = new List<Card>();

            foreach(var infoString in info.CardWithCount)
            {
                string[] vs = infoString.Split(' ');

                int count = int.Parse(vs[1]);
                for(int i=0;i<count;i++)
                {
                    deckCards.Add(CreatCardByName(vs[0]));
                }
            }

            return deckCards;
        }
        public Card CreatCardByName(string cardName)
        {
            // 在这里改版本号
            string path = "Infos/CardInfos/" + "V0.1/" + cardName+"/Card";
            var info = UnityEngine.Resources.Load<CardInfo2>(path);
            return CreatCardByInfo(info);
        }
        public Card CreatCardByInfo(CardInfo2 info)
        {
            if (info == null)
                return null;

            var card = new Card();

            //-------------基本信息-------------
            card.name = info.CardName;
            card.type = info.Type;
            card.Situation = CardSituation.Idle;
            card.cardLevel = info.UpChangeLevel;
            card.upChangeType = info.CardUpChangeType;
            card.cardElement = info.CardElement;

            //-------------action----------------
            card.CardAction = info.cardAction;

            //-------------effects---------------

            card.effects = new List<CardEffect>();

            foreach(var infoEffect in info.cardEffects)
            {
                var cloneEffect = infoEffect.Clone();

                cloneEffect.Card = card;
                foreach(var addEffect in cloneEffect.AdditionalEffects_List)
                {
                    addEffect.Card = card;
                }
                card.effects.Add(cloneEffect);
            }

            return card;
        }
    }
}
