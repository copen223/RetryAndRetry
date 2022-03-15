using System.Collections;
using System.Collections.Generic;
using ActorModule.Core;
using UnityEngine;

namespace BattleModule.BattleStates
{
    // 战斗初始化
    class BattleInit:BattleState
    {
        [Header("预设战斗人员列表")]
        public List<ActorController> ActorsInBattle_list = new List<ActorController>();

        public override void StateStart()
        {
            Manager.ActorList = ActorsInBattle_list;
            StartCoroutine(DelayEnterBattle(0.5f));
        }
        IEnumerator DelayEnterBattle(float time)
        {
            float timer = 0;
            while(timer < time)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            ChangeStateTo<BattleReady>();
        }
    }
}
