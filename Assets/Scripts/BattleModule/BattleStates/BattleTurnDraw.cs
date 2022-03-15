using ActorModule.Core;

namespace BattleModule.BattleStates
{
    class BattleTurnDraw : BattleState
    {
        public override void StateStart()
        {
            var actor = Manager.CurActorObject.GetComponent<ActorController>();

            if (GameManager.GameManager.instance.IfDebug)
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerDrawStart);
            else
            {
                if (actor.group.IsPlayer)
                    Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerDrawStart);
                else if (actor.group.IsEnemy)
                    ChangeStateTo<BattleTurnAction>();
            }
        }

        /// <summary>
        /// handController处理完毕抽卡后调用
        /// </summary>
        public void OnDrawOver()
        {
            if (Manager.currentState != this) return;
            ChangeStateTo<BattleTurnAction>();
        }
    }
}
