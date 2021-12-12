using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule
{
    public class CardBuilder
    {
        public List<Card> CreatCardsByDeckInfo(DeckInfo info)
        {
            List<Card> deckCards = new List<Card>();

            foreach(var infoString in info.CardWithCount)
            {
                string[] vs = infoString.Split(' ');

                int count = int.Parse(vs[1]);
                for(int i=0;i<count;i++)
                {
                    deckCards.Add(CreatCardByName(vs[0]));
                }
            }

            return deckCards;
        }
        public Card CreatCardByName(string cardName)
        {
            string path = "Infos/CardInfos/" + cardName;
            var info = Resources.Load<CardInfo>(path);
            return CreatCardByInfo(info);
        }
        public Card CreatCardByInfo(CardInfo info)
        {
            if (info == null)
                return null;

            var card = new Card();

            //-------------基本信息-------------
            card.name = info.CardName;
            card.type = info.Type;
            card.situation = CardSituation.Idle;
            card.cardLevel = info.UpChangeLevel;
            card.upChangeType = info.CardUpChangeType;
            card.cardElement = info.CardElement;
            
            //-------------action----------------
            List<float> actionValues = info.ActionValues;

            switch(info.ActionType)
            {
                case CardActionType.AttackTrail:card.CardAction = new CardActions.AttackTrail(actionValues[0],actionValues[1],(int)actionValues[2]);break;
                //case CardActionType.FocusTrail:card.CardAction = new CardActions.FocusTrail(actionValues[0],actionValues[1]);break;
                case CardActionType.None:card.CardAction = null;break;
            }

            //-------------effects---------------
            string[] effectStrings = info.Effects.Split(new char[] {';','；'});
            List<CardEffect> cardEffects = new List<CardEffect>();
            foreach(var effectString in effectStrings)
            {
                var effect = CreatEffectByText(effectString);
                if (effect != null)
                {
                    effect.Card = card;
                    cardEffects.Add(effect);
                }
            }

            card.effects = cardEffects;

            return card;
        }

        public CardEffect CreatEffectByText(string para)
        {
            CardEffect effect = null;

            //  对于拥有附加效果的词条处理
            string[] split_string_add = para.Split('-');
            
            if(split_string_add.Length>1)
            {
                para = split_string_add[0];

                var mainEffect = CreatEffectByText(para);

                for(int i=1;i<split_string_add.Length;i++)
                {
                    var addEffect = CreatEffectByText(split_string_add[i]);
                    mainEffect.AddAdditionalEffect(addEffect);
                }

                return mainEffect;
            }

            string[] split_strings = para.Split(new char[] { ':','：'});

            //字段格式：
            //效果名;
            //触发: 效果名;
            //触发: 效果名: 效果参数;

            //-附带效果;


            string trigger_string = "";   // 效果触发
            string name_string = "";      // 效果名
            string value_string = "0";     // 效果参量


            switch (split_strings.Length)
            {
                case 0: break;
                case 1: name_string = split_strings[0]; break;
                case 2: name_string = split_strings[0]; value_string = split_strings[1]; break;
                case 3: trigger_string = split_strings[0]; name_string = split_strings[1]; value_string = split_strings[2]; break;
            }

            EffectTrigger trigger = EffectTrigger.None;
            if (trigger_string != "")
            {
                switch (trigger_string)
                {
                    case "攻击":trigger = EffectTrigger.OnCombatAtk;break;
                    case "反应":trigger = EffectTrigger.OnCombatDfd;break;
                    case "无":trigger = EffectTrigger.None;break;
                    case "伤害":trigger = EffectTrigger.OnDoDamage;break;
                    case "打出":trigger = EffectTrigger.OnMake;break;
                }

            }


            switch (name_string)
            {
                case "伤害": case "普通伤害":case "NomalDamage":case "Damage":
                    effect = new CardEffects.NormalDamage(float.Parse(value_string),trigger);break;
                case "闪避": case "普通闪避": case "NomalDodge": case "Dodge":
                    effect = new CardEffects.NormalDodge(int.Parse(value_string));break;
                case "冲刺":
                    effect = new CardEffects.NormalDash(int.Parse(value_string));break;
                case "击退":
                    effect = new CardEffects.NormalBeatBack(int.Parse(value_string));break;
                case "格挡":
                    effect = new CardEffects.NormalBlock(int.Parse(value_string));break;
                case "毒":
                    effect = new CardEffects.GiveBuff(trigger, "毒");break;
                default: effect = null;break;
            }

            if (effect == null && name_string != "")
                Debug.LogError("该效果生成出错，请检查编写或代码：" + name_string);

            return effect;

        }
    }
}
