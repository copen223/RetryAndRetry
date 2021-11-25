using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools.NewTools;

public class UIManager : MonoBehaviour
{
    // 漂浮UI
    TargetPool floatUIPool;
    public GameObject floatUIPrefab;
    public Transform floatUIParent;

    public static UIManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        floatUIPool = new TargetPool(floatUIPrefab, floatUIParent);
    }


    #region 外部调用函数
    /// <summary>
    /// 在世界坐标的位置创建一个漂浮UI，向dir方向移动，time后消失。
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="dir"></param>
    /// <param name="time"></param>
    public GameObject CreatFloatUIAt(Vector3 worldPos,Vector2 dir,float time, Color color, string text)
    {
        var gb = floatUIPool.GetInstance();
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);
        gb.transform.position = screenPos;
        gb.transform.GetComponent<FloatUIController>().BaseWorldPos = worldPos;
        gb.transform.GetComponent<FloatUIController>().text.color = color;
        gb.transform.GetComponent<FloatUIController>().text.text = text;
        gb.GetComponent<FloatUIController>().StartMoveToDir(dir, time);

        return gb;
    }

    public GameObject CreatFloatUIAt(GameObject target, Vector2 dir, float time, Color color, string text)
    {
        var gb = floatUIPool.GetInstance();
        var screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        gb.transform.position = screenPos;
        gb.transform.GetComponent<FloatUIController>().text.color = color;
        gb.transform.GetComponent<FloatUIController>().text.text = text;
        gb.GetComponent<FloatUIController>().StartMoveToDir(target, dir, time);

        return gb;
    }
    #endregion

}
