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

    public GameObject AbillityUIPrefab;
    TargetPool abillityPool;
    public Transform abillityUIParent;

    // 
    void Awake()
    {
        
    }

    void Start()
    {
        fitsPool = new TargetPool(FitUIPrefab);

        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.ActorQueueCountChange, OnActorChanged);   // 添加监听

        abillityPool = new TargetPool(AbillityUIPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider == null)
                return;
            //Debug.Log(hit.collider.gameObject);

            //Debug.Log(hit.collider.transform.parent.tag);

            var screenUIPos = Camera.main.WorldToScreenPoint(worldPos += new Vector3(1, 0, 0));
            if (hit.collider.transform.parent.tag == "Actor")
            {

                //Debug.Log(hit.collider.transform.parent.tag);
                var con = hit.collider.transform.parent.GetComponent<ActorController>();
                OnShowActorAbillity(screenUIPos, con.Ability);
            }
        }
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

    private List<ActorAbility> actorAbilities = new List<ActorAbility>();
    public void OnShowActorAbillity(Vector3 ScreenPos,ActorAbility actorAbility)
    {
        if (actorAbilities.Contains(actorAbility))
            return;

        var gb = abillityPool.GetTarget(abillityUIParent);
        gb.transform.position = ScreenPos;

        var controller = gb.GetComponent<ActorAbilityUIController>();
        gb.SetActive(true);

        controller.UpdateValueByActor(actorAbility);

        controller.OnCloseWindowEvent += CloseActorAbillityUICallBack;
        actorAbilities.Add(actorAbility);

    }

    private void CloseActorAbillityUICallBack(ActorAbilityUIController who)
    {
        who.OnCloseWindowEvent -= CloseActorAbillityUICallBack;
        actorAbilities.Remove(who.ability);
    }
}
