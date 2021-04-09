using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class Deck : CardPool
    {
        public Deck(PlayerController holder,List<Card> cardList) : base(holder,cardList) { }

        public Card GetFirstCard()
        {
            if (list.Count <= 0)
            {
                Holder.discard.TranslateAllCardTo(this);
                Shuffle();
            }
            return list[0];

        }
        public Card GetFirstCard(CardType cardType)
        {
            for(int i =0;i<list.Count;i++)
            {
                var card = list[i];
                if (card.type == cardType)
                    return card;
            }
            Holder.discard.TranslateAllCardTo(this);
            Shuffle();
            for (int i = 0; i < list.Count; i++)
            {
                var card = list[i];
                if (card.type == cardType)
                    return card;
            }
            return null;
        }

        public void Shuffle()
        {
            Random random = new Random();

            for (int i = 0; i < list.Count; i++)
            {
                int t = random.Next(0, list.Count - 1);

                var temp = list[t];
                list[t] = list[i];
                list[i] = temp;
            }
        }
    }
}
