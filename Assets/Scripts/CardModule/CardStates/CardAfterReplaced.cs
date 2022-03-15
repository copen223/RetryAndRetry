using BattleModule;

namespace CardModule.CardStates
{
    class CardAfterReplaced : CardState
    {
        public override void StateStart()
        {
            base.StateStart();

            Controller.Card = Controller.ToBeReplacedCard;                  // 更新卡牌(这里应该改用发信息)
            Controller.SpriteObject.SendMessage("StartAnimation", 7);       // 旋转动画后段
            SetEventProtect();
        }

        public override void StateUpdate()
        {
          
        }

        public override void StateExit()
        {
            base.StateExit();

        }

        protected override void OnAnimationDo(bool isStart)
        {
            base.OnAnimationDo(isStart);
            if(!isStart)
            {
                if (Controller.currentState != this)
                    return;
                if (IsEventProtecting)
                    return;
                BattleManager.instance.SendMessage("OnDrawOver");
                ChangeStateTo<CardIdle>();
            }
        }
    }
}
