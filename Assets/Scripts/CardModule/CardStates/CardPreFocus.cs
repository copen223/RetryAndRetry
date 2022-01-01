using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardStates
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
        public void AddActionPointForPlayer()
        {
            if (Controller.Card.type == CardUseType.Passive)
                return;

            var actor = Controller.holder.GetComponent<ActorController>();
            if (actor is PlayerController)
            {
                var player = actor as PlayerController;
                player.ActionPoint += 1;
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
