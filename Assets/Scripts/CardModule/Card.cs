using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class Card
    {
        public string name;

        public Card(string _name,CardType _type,List<CardEffect> _effects)
        {
            name = _name;
            type = _type;
            effects = _effects;
        }

        public CardType type;
        public List<CardEffect> effects;
        public Container Container { get { return container; } set { container = value;if(value!=null && value.Card!=this)value.Card = this; } }
        private Container container;

        public virtual string GetCardDescription() { return ""; }
        
    }

    public enum CardType
    {
        Active,
        Passive
    }



}
