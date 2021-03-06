using System.Collections.Generic;
using CardModule;
using CardModule.CardActions;
using CardModule.CardEffects;
using UnityEngine;

namespace Resources.Infos
{
    [CreateAssetMenu(fileName = "Card", menuName = "MyInfo/卡牌")]
    public class CardInfo2 : ScriptableObject
    {
        public string CardName;
        public CardUseType Type;
        public int UpChangeLevel;
        public CardUpChangeType CardUpChangeType = CardUpChangeType.Normal;
        public CardElement CardElement = CardElement.Mu;

        public List<CardEffect> cardEffects = new List<CardEffect>();
        public CardAction cardAction;
    }
}
