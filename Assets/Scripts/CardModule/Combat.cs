using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CardModule;
using UnityEngine;
using Assets.Scripts.ActorModule;

namespace Assets.Scripts.CardModule
{
    public class Combat
    {
        // Combat的双方
        public ActorController Atker;
        public ActorController Dfder;

        public ActorCombatValue AtkerValue;
        public ActorCombatValue DfderValue;

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
            AtkerValue = new ActorCombatValue();
            DfderValue = new ActorCombatValue();
        }

        public Combat(Card atkCard,List<Card> dfdCards, GameObject atker,GameObject dfder)
        {
            AtkCard = atkCard;
            DfdCards = dfdCards;
            Atker = atker.GetComponent<ActorController>();
            Dfder = dfder.GetComponent<ActorController>();

            AtkerValue = new ActorCombatValue();
            DfderValue = new ActorCombatValue();

            AtkCard.User = Atker;
            foreach (var card in dfdCards) card.User = Dfder;
        }

        /// <summary>
        /// Combat进行时
        /// </summary>
        public void DoCombat()
        {
            //Atker.SetCombat(this);Dfder.SetCombat(this);

            //------------------载入攻击、防御卡牌的效果------------------
            CombatEffects.Clear();
            foreach(var effect in AtkCard.effects)
            {
                if(effect.Trigger == EffectTrigger.OnCombatAtk)
                {
                    effect.isAtking = true;
                    CombatEffects.Add(effect);
                }
                //if(effect.Trigger == EffectTrigger.OnDoDamage)
                //{
                //    effect.isAtking = true;
                //    Dfder.GetComponent<ActorController>().OnInjuredEvent += effect.DoEffect;
                //    ListenEventEffects.Add(effect);
                //}
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
                Debug.Log(effect.ToString() + "-" + i);
                if (!effect.IfInactive)
                {
                    effect.DoEffect(this);
                }
                effect.IfInactive = false;
            }

            //--------------------Combat执行结束----------------------
            //CancleAllListeners();
            CombatEndEvent?.Invoke();
        }

        /// <summary>
        /// 触发cards中所有卡牌触发为trigger的效果
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cards"></param>
        private void TouchOffEffect( EffectTrigger trigger, bool isAtking , params Card[] cards)
        {
            foreach(var card in cards)
            {
                foreach(var effect in card.effects)
                {
                    if(effect.Trigger == trigger)
                    {
                        effect.isAtking = isAtking;
                        if(!effect.IfInactive)
                            effect.DoEffect(this);
                        effect.IfInactive = false;
                    }
                }
            }
            
        }



        #region 一些效果处理接口
        public void DoHit(ActorController target,DamageData damageData)
        {
            // 伤害修正
            if(target == Atker)
            {
                damageData.damage *= AtkerValue.BeHitMutiValue;
            }
            else if(target == Dfder)
            {
                damageData.damage *= DfderValue.BeHitMutiValue;
            }

            // 闪避判断
            int dodge = target.Ability.Dodge.FinalValue;
            int targetHit = damageData.hit;
            int randomValue = UnityEngine.Random.Range(0, 12);

            Debug.Log("命中值" + targetHit + "随机值" + randomValue + "闪避值" + dodge);

            if (targetHit + randomValue < dodge)
            {
                target.OnDodge(damageData);
                return;
            }

            // 转身
            target.ChangeFaceTo(damageData.dir);

            // 受伤
            TouchOffEffect(EffectTrigger.OnDoDamage,true,AtkCard);
            target.OnInjured(damageData);
        }

        #endregion

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

        public class ActorCombatValue
        {
            public float BeHitMutiValue;

            public ActorCombatValue()
            {
                BeHitMutiValue = 1;
            }
        }

        public ActorCombatValue GetActorValue(ActorController actor)
        {
            if (actor == Atker)
                return AtkerValue;
            else
                return DfderValue;
        }

        public ActorController GetOtherActor(ActorController actor)
        {
            if (actor == Atker)
                return Dfder;
            else
                return Atker;
        }


    }

    
}
