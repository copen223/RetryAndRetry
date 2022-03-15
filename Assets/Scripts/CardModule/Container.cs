namespace CardModule
{
    public class Container
    {
        public CardUseType type;
        public Card Card{ get { return card; } set { card = value; if(value!=null&&card.Container!=this)card.Container = this; } }
        private Card card;
        
        public Container(CardUseType cardType)
        {
            type = cardType;
        }
    }
}
