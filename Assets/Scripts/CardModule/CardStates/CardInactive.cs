namespace CardModule.CardStates
{
    class CardInactive: CardState
    {
        public override void StateStart()
        {
            base.StateStart();

            Controller.SpriteObject.transform.localPosition = UnityEngine.Vector3.zero;
            Controller.SpriteObject.transform.localScale = UnityEngine.Vector3.one;
            gameObject.SetActive(false);
        }

        public override void StateUpdate()
        {

        }

        public override void StateExit()
        {
            base.StateExit();

            Controller.SpriteObject.SetActive(true);
        }

        
    }
}
