using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemController : MonoBehaviour
{
    public void SetAllButtonActive(bool ifAcive)
    {
        int childCount = transform.childCount;
        for(int i = 0; i<childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(ifAcive);
        }
    }
    
    public void OnEnterBattle()
    {
        SetAllButtonActive(true);
    }

    private void Start()
    {
        SetAllButtonActive(false);
        GameManager.instance.AddListener(GameManager.GameEvent.EnterBattle, OnEnterBattle);
    }

}
