﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleInit:BattleState
    {
        public List<GameObject> ActorsInBattle_list = new List<GameObject>();

        public override void StateStart()
        {
            Manager.ActorList = ActorsInBattle_list;
            ChangeStateTo<BattleStart>();
        }
    }
}
