using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;

public class ActorUIController : MonoBehaviour
{
    // 对象池
    public GameObject FitUIPrefab;
    TargetPool fitsPool;
    public Transform FitUIParent;

    // 
    void Awake()
    {
        fitsPool = new TargetPool(FitUIPrefab);
    }

    void Start()
    {
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.ActorQueueCountChange,OnActorChanged);   // 添加监听
    }


    // 暂时使用遍历整个队列 更新UI的方法，后续为了优化应该对Actor的增减进行监听，event委托应为含有一个Actor对象参数的委托
    void OnActorChanged()
    {
        List<GameObject> actors = BattleManager.instance.ActorQueue; // 读取列表
        fitsPool.ReSet();
        foreach(var actor in actors )   //  为每个actor创建一个fitui
        {
            var gb = fitsPool.GetTarget(FitUIParent);
            gb.SetActive(true);
            var controller = gb.GetComponent<ActorFitUIController>();
            controller.Target = actor;
        }
    }
}
