using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBattleButtonController : MonoBehaviour
{
    // 点击响应
    public void EnterBattle()
    {
        GameManager.instance.OnEnterBattle();
        gameObject.SetActive(false);
    }
}
