using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule
{
    public class DiscardPool:CardPool
    {
        public DiscardPool(PlayerController holder, List<Card> cardList) : base(holder,cardList) { }

        public void TranslateAllCardTo(CardPool pool)
        {
            int count = list.Count;
            for(int i=0;i<count;i++)
            {
                TranslateCardTo(list[0], pool);
            }
        }
    }
}
