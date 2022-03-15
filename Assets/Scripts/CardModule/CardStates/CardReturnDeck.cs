namespace CardModule.CardStates
{
    class CardReturnDeck : CardState
    {
        public override void StateStart()
        {
            //Controller.Card.situation = CardSituation.Idle;

            base.StateStart();
            int index = 5;
            Controller.SpriteObject.SendMessage("StartAnimation", index);
            Controller.gameObject.SetActive(false);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();
        }
    }
}
