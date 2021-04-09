using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleTurnAction : BattleState
    {
        void OnTurnEnd()
        {
            if (Manager.currentState != this)
                return;
            ChangeStateTo<BattleTurnEnd>();
        }
    }

    
}
