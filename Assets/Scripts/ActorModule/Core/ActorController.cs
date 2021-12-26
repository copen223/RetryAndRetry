using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;
using Assets.Scripts.ActorModule.ActorStates;
using Assets.Scripts.Tools;
using ActorModule.AI;
using Assets.Scripts.CardModule;

/// <summary>
/// 人物对象控制器，player和enemy控制器都继承自它，都实现了可交互对象接口
/// </summary>
public class ActorController : MonoBehaviour,ICanBeHitObject
{
    // 标签
    protected bool IfMyTurn { get { return BattleManager.instance.CurActorObject == gameObject; } }

    // 链接
    public BuffController BuffCon { get { return transform.Find("Buffs").GetComponent<BuffController>(); } }
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

    #region 其他对外方法
    /// <summary>
    /// 将坐标与网格对齐
    /// </summary>
    public void InitPosition()
    {
        Vector3Int cellPos = GetComponent<PathFinderComponent>().WorldToCell(transform.position);
        transform.position = GetComponent<PathFinderComponent>().CellToWorld((cellPos.x,cellPos.y));
    }
    #endregion


    #region 属性相关
    [Header("属性设置")]
    public float HealPoint;
    public float HealPoint_Max;

    public ActorAbility Ability;

    #endregion
    // 外部对象
    [HideInInspector]
    public GameObject FocusCountUI_GO;

    #region 贴图控制
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

    #region 专注轨迹相关

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
    public void RemoveFocusTrail(GameObject gb,bool ifDestory) { focusTrails.Remove(gb); gb.GetComponent<FocusTrailController>().Actor = null; EventInvoke(ActorEvent.OnFoucusTrailChange); if (ifDestory) Destroy(gb); }

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
    /// 激活focustrail，但不显示。一般只在被选作攻击对象时才会激活focustrail;所以在回合结束时会将所有trail暂时失活。
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
        if (FocusCountUI_GO == null) return;
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
        ActiveAllFocusTrail(false);
    }
    /// <summary>
    /// 回合开始时触发
    /// </summary>
    public virtual void OnTurnStart()
    {
        
    }

    #endregion

    #region 状态机与事件相关
    [Header("状态机与事件相关，脚本中进行初始化")]
    public ActorState currentState;
    public List<ActorState> ActorStates = new List<ActorState>();
    #endregion



    #region 效果处理相关
    //private Combat combat;
    //public void SetCombat(Combat combat) { this.combat = combat; }

    //public event System.Action<Combat> OnInjuredEvent;


    #endregion

    #region 事件相关
    // 事件响应

    private List<float> damageDatasList = new List<float>();

    /// <summary>
    /// 决定Onbehit时的演出表现
    /// </summary>
    [HideInInspector]
    public bool ifBlock = false;

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
        float time = 1f;
        UIManager.instance.CreatFloatUIAt(Sprite, textMove, time, Color.green, "闪避");
    }

    private void OnBlock(DamageData data)
    {
        // 格挡字符
        Vector3 textOffset = new Vector3(0, 0.25f, 0);
        Vector3 textRandomOffset = new Vector3(Random.Range(0, 0.5f), Random.Range(0, 0.5f), 0);
        Vector3 textPos = Sprite.transform.position + textOffset + textRandomOffset;
        Vector3 textMoveOffset = new Vector3(0, 0.5f, 0);
        Vector3 textMove = new Vector3(data.dir.x / 4, 1f, 0);
        float time = 1f;
        UIManager.instance.CreatFloatUIAt(Sprite, textMove, time, Color.blue, "格挡");
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void OnInjured(DamageData data)
    {
        // 伤害应用
        var damage = data.damage - Ability.Defense.FinalValue;
        if (damage < 0)
            damage = 0;

        if (HealPoint - damage <= 0)
            damage = HealPoint;
        HealPoint -= damage;

        // 如果存在格挡，则显示格挡
        if (ifBlock)
        {
            OnBlock(data);
        }

        // 伤害字符
        Vector3 textOffset = new Vector3(0, 0.25f, 0);
        Vector3 textRandomOffset = new Vector3(Random.Range(0, 0.5f), Random.Range(0, 0.5f), 0);
        Vector3 textPos = Sprite.transform.position + textOffset + textRandomOffset;
        Vector3 textMoveOffset = new Vector3(0, 0.5f, 0);
        Vector3 textMove = new Vector3(-data.dir.x + textRandomOffset.y, 1f, 0);
        float time = 1f;
        UIManager.instance.CreatFloatUIAt(Sprite, textMove, time, Color.red, damage + "");

        //OnInjuredEvent?.Invoke(combat);

        EventInvoke(ActorEvent.OnBehit);
    }

    public void OnBeatBack(Vector2 dir,int dis)
    {
        var finder = GetComponent<PathFinderComponent>();
        var move = GetComponent<ActorMoveComponent>();

        var path = finder.SearchAndGetPathByEnforcedMove(transform.position, dir, dis, true);
        move.StartForceMoveByPathList(finder.VectorPath2NodePath(path));
    }

    public void OnFallDown()
    {
        var finder = GetComponent<PathFinderComponent>();
        var move = GetComponent<ActorMoveComponent>();

        var path = finder.SearchAndGetPathByFallDown(transform.position, FaceDir.x > 0);
        move.StartForceMoveByPathList(finder.VectorPath2NodePath(path));
    }

    public void OnBuff(string buff)
    {
        Debug.Log(gameObject + "" + buff);
    }

    public void OnDoAttack()
    {
        var con = gameObject.GetComponent<ActorController>();

        CardBuilder2 builder = new CardBuilder2();

        var weaknessCard = builder.CreatCardByName("弱点");

        float angle = Random.Range(0, 360f);
        Vector3 referenceDir = Vector3.right;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 dir3D = (Vector3)referenceDir;
        Vector3 thisDir = rotation * dir3D;

        if (con is PlayerController)
        {
            var player = con as PlayerController;
            player.AddCardToHand(weaknessCard);
            player.FocusOneCard(weaknessCard, thisDir);
        }
        else if(con is EnemyController)
        {
            var enemy = con as EnemyController;
            enemy.FocusOneCard(weaknessCard, thisDir);
        }
    }

    #endregion

    #region 遗留的老事件系统


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



