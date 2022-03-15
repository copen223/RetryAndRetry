namespace CardModule.CardStates
{
    class CardPreReplaced : CardState
    {
        public override void StateStart()
        {
            base.StateStart();
            Controller.SpriteObject.SendMessage("StartAnimation", 6);
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
            //Debug.Log(name);
            if (!isStart)
            {
                if (Controller.currentState != this)
                    return;
                ChangeStateTo<CardAfterReplaced>();
            }


        }

    }
}
