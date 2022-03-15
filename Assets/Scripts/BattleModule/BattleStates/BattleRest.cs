namespace BattleModule.BattleStates
{
    // 闲置状态
    class BattleRest:BattleState
    {
        public override void StateStart()
        {
            GameManager.GameManager.instance.AddListener(GameManager.GameManager.GameEvent.EnterBattle, OnEnterBattle);
        }

        private void OnEnterBattle()
        {
            ChangeStateTo<BattleInit>();
        }
    }
}
