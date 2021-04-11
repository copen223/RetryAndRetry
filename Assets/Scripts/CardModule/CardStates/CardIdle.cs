﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Assets.Scripts.CardModule.CardStates
{
    class CardIdle: CardState,IPointerEnterHandler
    {
        public override void StateStart()
        {
            base.StateStart();
            int index = 1;
            Controller.SpriteObject.SendMessage("StartAnimation", index);
        }

        public override void StateUpdate()
        {
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Controller.currentState != this)
                return;
            ChangeStateTo<CardSelected>();
        }
    }
}