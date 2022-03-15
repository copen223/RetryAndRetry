using ActorModule.Core;

namespace BattleModule.BattleStates
{
    class BattleTurnEnd : BattleState
    {
        public override void StateStart()
        {
            var actor = Manager.CurActorObject.GetComponent<ActorController>();
            actor.OnTurnEnd();

            Manager.CurActorIndex += 1;
            if (Manager.CurActorIndex >= Manager.ActorQueue.Count) Manager.CurActorIndex = 0;

            Manager.InvokeTurnEndEventEventByState();
            
            ChangeStateTo<BattleTurnStart>();
            // 一轮结束
        }
    }
}
