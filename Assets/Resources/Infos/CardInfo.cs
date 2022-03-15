using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using CardModule;
using UnityEngine;
using UnityEngine;

namespace Resources.Infos
{
    [CreateAssetMenu(fileName = "Card", menuName = "MyInfo/旧方法/卡牌")]
    public class CardInfo : ScriptableObject
    {
        public string CardName;
        public CardUseType Type;
        public int UpChangeLevel;
        public CardUpChangeType CardUpChangeType = CardUpChangeType.Normal;
        public CardElement CardElement = CardElement.Mu;
        public string Effects;
        public CardActionType ActionType;
        public List<float> ActionValues= new List<float>();
    }

    public enum CardActionType { None,AttackTrail,FocusTrail}
}