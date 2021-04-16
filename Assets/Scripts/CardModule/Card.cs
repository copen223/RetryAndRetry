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
            situation = CardSituation.Idle;
        }

        public CardType type;               // 卡牌类型 主动型/被动型 主动型打出时会生效，被动型专注时会生效。
        public CardSituation situation;     // 表明卡牌在手牌中的状态，刷新时根据该项来决定显示层状态
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

    public enum CardSituation
    {
        Idle,
        Focused
    }




}
