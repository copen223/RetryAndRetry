using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;
using Assets.Scripts.ActorModule.ActorStates;
using Assets.Scripts.Tools;
using ActorModule.AI;

public class ActorController : MonoBehaviour
{
    // 链接
    public GameObject Sprite { get { return transform.Find("Sprite").gameObject; } }
    public Vector3 CenterOffset { get { return Sprite.transform.localPosition; } }
    public Vector3 CenterPos { get { return transform.position + CenterOffset; } }

    public int advantage;   // 先攻
    public ActorGroup group;
    /// <summary>
    /// sprite在scale为1时面朝方向是否为右边，用于移动时切换朝向判断
    /// </summary>
    [Header("设置标签")]
    [SerializeField]
    private bool SpriteFaceToRight = false;
    public Vector2 FaceDir 
    { 
        get 
        {
            if (SpriteFaceToRight)
                return transform.localScale.x * Vector2.right;
            else
                return transform.localScale.x * Vector2.left;
        }
    }


    // 属性
    [Header("属性设置")]
    public float HealPoint;
    public float HealPoint_Max;

    // 外部对象
    [HideInInspector]
    public GameObject FocusCountUI_GO;

    #region 贴图控制
    public void ChangeFaceTo(Vector3 dir) 
    {
        // 方向接近于0，不改变方向
        if (Mathf.Abs( dir.x) <= Mathf.Epsilon)
            return;
        int faceValue = SpriteFaceToRight ? 1 : -1;
        int scale_x = (dir * faceValue).x >= 0 ? 1 : -1;
        transform.localScale = new Vector3(scale_x, transform.localScale.y, transform.localScale.z);
    }
    #endregion



    /// <summary>
    /// 专注轨迹数目
    /// </summary>
    public int FocusTrailCount { get { return focusTrails.Count; } }

    public virtual void OnEnterBattle()
    { }
    
    /// <summary>
    /// 开始执行战斗回合的ai
    /// </summary>
    public  void StartDoBattleAI()
    {
        var ai_go = transform.Find("AI").gameObject;
        if(ai_go==null)
        {
            Debug.Log(gameObject + "该对象不存在战斗AI");
            return;
        }
        ActionModeInBattle am = ai_go.GetComponent<ActionModeInBattle>();
        am.StartDoBattleAI();
    }

    #region 卡牌战斗相关

    //-----------------FocusTrail---------------------//
    [SerializeField]
    [Header("专注轨迹相关，不需要设置")]
    private List<GameObject> focusTrails = new List<GameObject>();  // 该单位存在的专注轨迹

    /// <summary>
    /// 给该对象添加专注轨迹，但是轨迹的卡牌尚未设定
    /// </summary>
    /// <param name="gb"></param>
    public void AddFocusTrail(GameObject gb) { focusTrails.Add(gb); gb.GetComponent<FocusTrailController>().Actor = gameObject; gb.transform.parent = transform; EventInvoke(ActorEvent.OnFoucusTrailChange); }
    /// <summary>
    /// 将专注轨迹与自己解绑
    /// </summary>
    /// <param name="gb"></param>
    public void RemoveFocusTrail(GameObject gb) { focusTrails.Remove(gb); gb.GetComponent<FocusTrailController>().Actor = null; EventInvoke(ActorEvent.OnFoucusTrailChange); }
    
    /// <summary>
    /// 该方法一般由AI调用
    /// 玩家专注轨迹的变化和卡牌变化绑定，卡牌在从专注状态退出时会调用上方的Remove方法
    /// 而AI不会，所以提供该方法在回合开始时清空AI的专注轨迹,
    /// 相比之下会把卡牌与专注轨迹解绑 并且destroy轨迹
    /// </summary>
    public void RemoveAllFocusTrail() 
    {
        foreach (var gb in focusTrails) 
        { 
            gb.GetComponent<FocusTrailController>().Actor = null;
            gb.GetComponent<FocusTrailController>().Card?.CancleFocusTrail();
            Destroy(gb);
            EventInvoke(ActorEvent.OnFoucusTrailChange); 
        } 
        
        focusTrails.Clear(); 
    }
    
    /// <summary>
    /// 显示该单位的所有专注轨迹
    /// </summary>
    /// <param name="isActive"></param>
    public void ShowAllFocusTrail(bool isActive)
    {
        foreach (var gb in focusTrails)
        {
            gb.GetComponent<FocusTrailController>().IfShow = isActive;
        }
    }

    /// <summary>
    /// 激活focustrail，但不显示
    /// </summary>
    /// <param name="isActive"></param>
    public void ActiveAllFocusTrail(bool isActive)
    {
        foreach (var gb in focusTrails) gb.SetActive(isActive);
    }

    /// <summary>
    /// 显示一定数目的轨迹
    /// </summary>
    public void ShowFocusTrail(int num,bool isActive)
    {
        for(int i = 0; i < num; i++) 
        { 
            if (i < focusTrails.Count) 
                focusTrails[i].GetComponent<FocusTrailController>().IfShow = isActive; 
        }
    }

    /// <summary>
    /// 显示该单位目前的专注轨迹数目
    /// </summary>
    public void ShowFocusTrailCount(bool isActive)
    {
        FocusCountUI_GO.SetActive(isActive);
    }

    //----------------FocusTrail-END------------------//

    #endregion

    #region 流程响应事件
    /// <summary>
    /// 回合结束时触发
    /// </summary>
    public virtual void OnTurnEnd()
    {
        //if (BattleManager.instance.CurActorObject == gameObject)
        //{
        currentState.ChangeStateTo<ActorNoActionIdle>();
        //}
    }
    /// <summary>
    /// 回合开始时触发
    /// </summary>
    public virtual void OnTurnStart()
    {
        //if (BattleManager.instance.CurActorObject == gameObject)
        //{
        currentState.ChangeStateTo<ActorActionIdle>();
        //}
    }

    #endregion

    #region 状态机与事件相关
    [Header("状态机与事件相关，脚本中进行初始化")]
    public ActorState currentState;
    public List<ActorState> ActorStates = new List<ActorState>();
    #endregion

    #region 事件相关
    // 事件响应

    private List<float> damageDatasList = new List<float>();
    public void OnBehit(DamageData data)
    {
        var damage = data.damage;
        if (HealPoint - damage <= 0)
            damage = HealPoint;
        HealPoint -= damage;

        // 转身
        ChangeFaceTo(data.dir);

        // 伤害字符
        Vector3 textOffset = new Vector3(0, 0.25f, 0);
        Vector3 textRandomOffset = new Vector3(Random.Range(0,0.5f), Random.Range(0,0.5f), 0);
        Vector3 textPos = Sprite.transform.position + textOffset + textRandomOffset;
        Vector3 textMoveOffset = new Vector3(0, 0.5f, 0);
        Vector3 textMove = new Vector3(-data.dir.x, 1f, 0);
        float time = 0.5f;
        UIManager.instance.CreatFloatUIAt(textPos, Vector3.zero, time, Color.red, data.damage + "");

        EventInvoke(ActorEvent.OnBehit);
    }
    public void OnHealUp(float heal)
    {
        if(HealPoint + heal >= HealPoint_Max)
            heal = HealPoint;
        HealPoint += heal;
        EventInvoke(ActorEvent.OnHealUp);
    }

    public void OnDodge(DamageData data)
    {
        // 转身
        ChangeFaceTo(data.dir);

        // 闪避字符
        Vector3 textOffset = new Vector3(0, 0.25f, 0);
        Vector3 textRandomOffset = new Vector3(Random.Range(0, 0.5f), Random.Range(0, 0.5f), 0);
        Vector3 textPos = Sprite.transform.position + textOffset + textRandomOffset;
        Vector3 textMoveOffset = new Vector3(0, 0.5f, 0);
        Vector3 textMove = new Vector3(-data.dir.x/2, 1f, 0);
        float time = 0.5f;
        UIManager.instance.CreatFloatUIAt(textPos, Vector3.zero, time, Color.green, "闪避");
    }

    public void OnBeatBack(Vector2 dir,int dis)
    {
        var finder = GetComponent<PathFinderComponent>();
        var move = GetComponent<ActorMoveComponent>();

        var path = finder.SearchAndGetPathByEnforcedMove(transform.position, dir, dis, true);
        move.StartForceMoveByPathList(finder.VectorPath2NodePath(path));
    }

    //-------------------事件订阅-------------------//
    public enum ActorEvent
    {
        OnBehit,
        OnHealUp,
        OnFoucusTrailChange,
        OnBattleSelected
    }
    public event ActorHandle EventObserver_OnBehit;
    public event ActorHandle EventObserver_OnHealUp;
    public event ActorHandle EventObserver_OnFoucusTrailChange;
    public event ActorHandle EventObserver_OnBattleSelected;


    public void AddEventObserver(ActorEvent actorEvent, ActorHandle handle)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit += handle; break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp += handle; break;
            case ActorEvent.OnFoucusTrailChange: EventObserver_OnFoucusTrailChange += handle;break;
            case ActorEvent.OnBattleSelected:EventObserver_OnBattleSelected += handle;break;
        }
    }
    public void RemoveEventObserver(ActorEvent actorEvent, ActorHandle handle)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit -= handle; break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp -= handle; break;
            case ActorEvent.OnFoucusTrailChange: EventObserver_OnFoucusTrailChange -= handle; break;
            case ActorEvent.OnBattleSelected: EventObserver_OnBattleSelected -= handle; break;
        }
    }

    public void EventInvoke(ActorEvent actorEvent)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit?.Invoke(gameObject); break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp?.Invoke(gameObject); break;
            case ActorEvent.OnFoucusTrailChange: EventObserver_OnFoucusTrailChange?.Invoke(gameObject); break;
            case ActorEvent.OnBattleSelected:EventObserver_OnBattleSelected?.Invoke(gameObject);break;
        }

    }

    //-----------------事件数据类型定义-------------------//
    public delegate void ActorHandle(GameObject actor);
    public delegate void HealHandle(HealData healData);
    public class HealData { public HealData(GameObject atk, GameObject dfd) { atker = atk;dfder = dfd;} GameObject atker; GameObject dfder; }

    #endregion
}

public class DamageData
{
    public float damage;
    public Vector2 dir;
    public DamageData(float _damage,Vector2 _dir)
    {
        damage = _damage;
        dir = _dir;
    }
}


