using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CardModule.CardActions;
using UnityEngine;

namespace Assets.Scripts.CardModule
{
    public class Card
    {
        /// <summary>
        /// 卡牌名称
        /// </summary>
        public string name;

        /// <summary>
        /// 该卡牌的所属者
        /// </summary>
        public ActorController User;

        public Card() { }
        public Card(string _name,CardUseType _type,List<CardEffect> _effects)
        {
            name = _name;
            type = _type;
            effects = _effects;
            situation = CardSituation.Idle;
            upChangeType = CardUpChangeType.Normal;
            cardLevel = 0;
        }
        public Card(string _name, CardUseType _type, List<CardEffect> _effects,CardAction action)
        {
            name = _name;
            type = _type;
            effects = _effects;
            situation = CardSituation.Idle;
            upChangeType = CardUpChangeType.Normal;
            cardLevel = 0;
            CardAction = action;
        }

        public Card(string _name, CardUseType _type, List<CardEffect> _effects, CardAction action,int upChangeLevel)
        {
            name = _name;
            type = _type;
            effects = _effects;
            situation = CardSituation.Idle;
            upChangeType = CardUpChangeType.Normal;
            cardLevel = upChangeLevel;
            CardAction = action;
        }


        public CardUseType type;               // 卡牌类型 主动型/被动型 主动型打出时会生效，被动型专注时会生效
        public CardSituation situation;     // 表明卡牌在手牌中的状态，刷新时根据该项来决定显示层状态
        public GameObject focusTrail;       // 卡牌专注轨迹，用途只有判断该卡牌是否有专注轨迹 不影响使用
        public CardUpChangeType upChangeType;
        public int cardLevel;
        public CardElement cardElement;

        /// <summary>
        /// 仅仅显示该卡牌的专注轨迹，没有实际作用
        /// </summary>
        /// <param name="ifShow"></param>
        public void ShowFocusTrail(bool ifShow)
        {
            if(focusTrail != null)
                focusTrail.GetComponent<FocusTrailController>().IfShow = ifShow;
        }

        /// <summary>
        /// 设置专注轨迹，同时把专注轨迹的卡牌设置为该卡牌
        /// 即链接彼此
        /// </summary>
        /// <param name="gb"></param>
        public void SetFocusTrail(GameObject gb)
        {
            focusTrail = gb;
            gb.GetComponent<FocusTrailController>().Card = this;
        }
        /// <summary>
        /// 取消专注轨迹，即把两者的链接全部设为null,解绑
        /// </summary>
        /// <returns></returns>
        public GameObject CancleFocusTrail()
        {
            var gb = focusTrail;
            focusTrail.GetComponent<FocusTrailController>().Card = null;
            focusTrail = null;
            return gb;
        }
        public bool IfHasTrail { get { return focusTrail != null; } }

        public List<CardEffect> effects;    // 卡牌效果列表
        public CardAction CardAction;       // 卡牌如何使用，是射线选择，还是直接选择目标，还是自动完成
        //----------卡槽-------------
        public Container Container { get { return container; } set { container = value;if(value!=null && value.Card!=this)value.Card = this; } }
        private Container container;
        //---------------------------
        public virtual string GetCardDescription() 
        {
            string text = "";
            foreach(var effect in effects)
            {
               text += effect.GetDescriptionText()+" ";
            }
            return text;
        }
        
    }

    public enum CardUseType
    {
        Active = 1,
        Passive = 2
    }

    public enum CardSituation
    {
        Idle,
        Focused
    }

    public enum CardUpChangeType
    {
        Normal,
        UpChange
    }

    public enum FoucsPos
    {
        ForwardUp,
        ForwardDown,
        BackUp,
        BackDown
    }

    public enum CardElement
    {
        Jin,
        Mu,
        Shui,
        Huo,
        Tu
    }
}
