using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

[CreateAssetMenu(fileName = "name", menuName = "MyInfo/卡牌")]
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
