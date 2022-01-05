using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStatusUI_Ability : MonoBehaviour
{
    public Text Attack;
    public Text Dfence;
    public Text Hit;
    public Text Dodge;
    [HideInInspector]
    public ActorAbility ability;

    /// <summary>
    /// 更新该UI的数值显示
    /// </summary>
    /// <param name="actorAbility"></param>
    public void UpdateValueByActor(ActorAbility actorAbility)
    {
        ability = actorAbility;
        Attack.text = actorAbility.Attack.FinalValue.ToString();
        Dfence.text = actorAbility.Defense.FinalValue.ToString();
        Hit.text = actorAbility.Hit.FinalValue.ToString();
        Dodge.text = actorAbility.Dodge.FinalValue.ToString();
    }
}
