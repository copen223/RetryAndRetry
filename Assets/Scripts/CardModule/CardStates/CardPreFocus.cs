using ActorModule.Core;
using CardModule.CardEffects;

namespace CardModule.CardStates
{
    /// 专注动画状态1
    class CardPreFocus : CardState
    {
        public override void StateStart()
        {
            base.StateStart();

            AddActionPointForPlayer(); // 为玩家增加1点行动点数

            Controller.SpriteObject.SendMessage("StartAnimation", 8);

            SetEventProtect();
            TouchOffEffectOnFocus();
        }

        /// <summary>
        /// 打出阴牌时的点数返还
        /// </summary>
        public void AddActionPointForPlayer()
        {
            if (Controller.Card.type == CardUseType.Passive)
                return;

            if (Controller.holder.TryGetComponent(out PlayerController player))
            {
                player.ActionPoint += Controller.Card.cardLevel;
            }
        }

        private void TouchOffEffectOnFocus()
        {
            var card = Controller.Card;

            foreach(var effect in card.effects)
            {
                if(effect.Trigger == EffectTrigger.OnFocus)
                {
                    effect.DoEffect(new Combat(card.User, card.User));
                }
            }
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
                ChangeStateTo<CardFocus>();
            }
        }
    }
}
