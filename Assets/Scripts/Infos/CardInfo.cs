using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

[CreateAssetMenu(fileName = "name", menuName = "MyInfo/卡牌")]
public class CardInfo : ScriptableObject
{
    public string CardName;
    public CardType Type;
    public string Effects;
    public CardActionType ActionType;
    public List<float> ActionNum = new List<float>();
}

public enum CardActionType { None,BattleTrail,FocusTrail}
