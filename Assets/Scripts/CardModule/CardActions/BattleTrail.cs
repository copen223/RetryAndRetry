using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardModule.CardActions
{
    public class BattleTrail : CardAction
    {
        float distance_max;
        float distance_min;
        public BattleTrail(float min,float max)
        {
            distance_min = min;
            distance_max = max;
        }
    }
}
