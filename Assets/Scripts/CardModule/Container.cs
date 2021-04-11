using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class Container
    {
        public CardType type;
        public Card Card{ get { return card; } set { card = value; if(value!=null&&card.Container!=this)card.Container = this; } }
        private Card card;
        
        public Container(CardType cardType)
        {
            type = cardType;
        }
    }
}
