using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    #region 链接
    public GameObject Hand;
    public GameObject SelectionWindow;
    #endregion

    #region 单例
    public static CardSystem instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    #endregion
}
