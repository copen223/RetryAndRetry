using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CardModule;
using UnityEngine;

namespace Assets.Scripts.CardModule
{
    public class Combat
    {
        // Combat的双方
        public ActorController Atker;
        public ActorController Dfder;

        // 待处理Combat效果列表
        public List<CardEffect> CombatEffects = new List<CardEffect>();

        public List<Card> DfdCards = new List<Card>();
        public Card AtkCard;

        // 待处理特殊条件效果列表
        public List<CardEffect> ListenEventEffects = new List<CardEffect>();


        #region Combat事件
        public event Action CombatEndEvent;
        #endregion


        public Combat (ActorController atker,ActorController dfder)
        {
            Atker = atker;
            Dfder = dfder;
        }

        public Combat(Card atkCard,List<Card> dfdCards, GameObject atker,GameObject dfder)
        {
            AtkCard = atkCard;
            DfdCards = dfdCards;
            Atker = atker.GetComponent<ActorController>();
            Dfder = dfder.GetComponent<ActorController>();

            AtkCard.User = Atker;
            foreach (var card in dfdCards) card.User = Dfder;
        }

        /// <summary>
        /// Combat进行时
        /// </summary>
        public void DoCombat()
        {
            Atker.SetCombat(this);Dfder.SetCombat(this);

            //------------------载入攻击、防御卡牌的效果------------------
            CombatEffects.Clear();
            foreach(var effect in AtkCard.effects)
            {
                if(effect.Trigger == EffectTrigger.OnCombatAtk)
                {
                    effect.isAtking = true;
                    CombatEffects.Add(effect);
                }
                if(effect.Trigger == EffectTrigger.OnDoDamage)
                {
                    effect.isAtking = true;
                    Dfder.GetComponent<ActorController>().OnInjuredEvent += effect.DoEffect;
                    ListenEventEffects.Add(effect);
                }
            }
            foreach (var card in DfdCards)
            {
                foreach (var effect in card.effects)
                {
                    effect.isAtking = false;
                    CombatEffects.Add(effect);
                }
            }
            //--------------------效果载入完成------------------

            CombatEffects.Sort(new EffectSortByPriority()); // 按Combat优先级给效果排序

            for(int i = 0;i<CombatEffects.Count;i++)    // 效果依次对该Combat进行处理
            {
                var effect = CombatEffects[i];
                effect.DoEffect(this);
            }

            //--------------------Combat执行结束----------------------
            CancleAllListeners();
            CombatEndEvent?.Invoke();
        }

        /// <summary>
        /// combat已经结束，注销所有有监听的效果
        /// </summary>
        private void CancleAllListeners()
        {
            foreach(var effect in ListenEventEffects)
            {
                ActorController user = effect.Card.User;
                ActorController target = effect.isAtking ? Dfder.GetComponent<ActorController>() : Atker.GetComponent<ActorController>();
                switch(effect.Trigger)
                {
                    case EffectTrigger.OnDoDamage:
                        target.OnInjuredEvent -= effect.DoEffect; break;
                }
            }
        }


        public void StartDoCombat()
        {
            DoCombat();
        }



        private class EffectSortByPriority : IComparer<CardEffect>
        {
            public int Compare(CardEffect x, CardEffect y)
            {
                if (x.CombatPriority > y.CombatPriority)
                    return -1;
                else if (x.CombatPriority == y.CombatPriority)
                    return 0;
                else
                    return 1;
            }
        }

    }

    
}
