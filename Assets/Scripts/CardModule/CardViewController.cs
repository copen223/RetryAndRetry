using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour
{
    //-------------------------------------------事件定义-----------------------------------------------//

    public delegate void BoolHandler(bool isStart);
    public event BoolHandler AnimationEventObserver;
    private void OnAnimationOver()
    {
        AnimationEventObserver?.Invoke(false);
    }
    private void OnAnimationStart()
    {
        AnimationEventObserver?.Invoke(true);
    }

    public void AddOberserver(BoolHandler handler,object who) { AnimationEventObserver += handler; /*Debug.Log("监听者添加：" + who.ToString());*/ }
    public void RemoveOberserver(BoolHandler handler,object who) { AnimationEventObserver -= handler; /*Debug.Log("监听者移出：" + who.ToString());*/ }

    //--------------------------------------------------------------------------------------------------//
    //-----------------------------------------卡牌动画开始---------------------------------------------//
    //--------------------------------------------------------------------------------------------------//
    public CardController controller;
    [Header("选中动画")]
    public float Time_Animation_Selected;
    public float OffsetY_Animation_selected;
    [Header("平躺动画")]
    public float Time_Animation_FallDown;
    [Header("移动动画")]
    public float Time_Animation_Move;
    [Header("旋转动画")]
    public float Time_Animation_Rotate;

    bool canBreakAnimation = true;


    public Transform DeckTransform;
    public Transform DiscardTransform;

    private void Start()
    {
        controller = transform.parent.GetComponent<CardController>();
        DeckTransform = controller.transform.parent.transform.parent.GetComponent<HandController>().DeckObject.GetComponent<Transform>();
        DiscardTransform = controller.transform.parent.transform.parent.GetComponent<HandController>().DiscardObject.GetComponent<Transform>();
    }

    public void StartAnimation(int index)
    {
        if (!canBreakAnimation)
            return;

        StopAllCoroutines();
        animations_list = new List<IEnumerator> { SelectedAnimation(true), SelectedAnimation(false),FallDownAnimation(true), FallDownAnimation(false),
        MoveToCardPoolAnimation(DiscardTransform.position),MoveToCardPoolAnimation(DeckTransform.position)
        ,ReplaceAnimation(true),ReplaceAnimation(false)};

        StartCoroutine(animations_list[index]);
        OnAnimationStart();
    }

    public void OnReset()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        OnAnimationOver();
    }

    public void SetFocusRotation()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(transform.localPosition.x, OffsetY_Animation_selected, transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        OnAnimationOver();
    }


    private List<IEnumerator> animations_list;

    delegate IEnumerator GetEnumeator(bool i);

    // 选中时动画，可打断，移动模式为local，围绕card parent移动
    IEnumerator SelectedAnimation(bool isSelected)
    {
        float startY = isSelected ? 0 : OffsetY_Animation_selected;
        float endY = isSelected ? OffsetY_Animation_selected : 0;
        float curY = transform.localPosition.y;
        float timer = 0;

        while(curY != endY)
        {
            curY = Mathf.Lerp(startY, endY, timer/Time_Animation_Selected);
            transform.localPosition = new Vector3(0, curY,transform.localPosition.z);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        OnAnimationOver();
    }
    // 专注时动画，不可打断
    IEnumerator FallDownAnimation(bool isFall)
    {
       // Debug.LogError(isFall);
        canBreakAnimation = false;
        float startZ = isFall ? 0 : 90;
        float endZ = isFall ? 90 : 0;
        float curZ = transform.localRotation.z;
        float timer = 0;

        float startY = 0 ;
        float endY = OffsetY_Animation_selected;
        float curY = transform.localPosition.y;

        while (curZ != endZ || curY != endY)
        {
            curZ = Mathf.LerpAngle(startZ, endZ, timer / Time_Animation_FallDown);
            Vector3 rotate = new Vector3(transform.localRotation.x, transform.localRotation.y, curZ);
            transform.localRotation = Quaternion.Euler(rotate);
            curY = Mathf.Lerp(startY, endY, timer / Time_Animation_Selected);
            transform.localPosition = new Vector3(transform.localPosition.x, curY, transform.localPosition.y);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canBreakAnimation = true;

        OnAnimationOver();
    }
    // 移动动画
    IEnumerator MoveToPos(Vector3 pos)
    {
        canBreakAnimation = false;
        float curY, curX,timer;
        float startX = curX = transform.localPosition.x;
        float startY = curY = transform.localPosition.y;
        timer = 0;
        while (curX!=pos.x||curY!=pos.y)
        {
            if(startX == pos.x && startY == pos.y) break;
            curX = Mathf.Lerp(startX, pos.x, timer / Time_Animation_Move);
            curY = Mathf.Lerp(startY, pos.y, timer / Time_Animation_Move);

            transform.localPosition = new Vector3(curX, curY, 0);
            yield return new WaitForEndOfFrame();
        }
        canBreakAnimation = true;
    }
    // 替换动画
    IEnumerator ReplaceAnimation(bool isBefore)
    {
        canBreakAnimation = false;
        float startY = isBefore ? 0 : 180;
        float endY = isBefore ? 180 : 360;
        float curY = transform.localRotation.y;
        float timer = 0;

        while (curY != endY)
        {
            curY = Mathf.LerpAngle(startY, endY, timer / Time_Animation_Rotate);
            Vector3 rotate = new Vector3(transform.localRotation.x, curY, transform.localRotation.z );
            transform.localRotation = Quaternion.Euler(rotate);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canBreakAnimation = true;

        OnAnimationOver();
    }

    private Vector3 Vector3Lerp(Vector3 startPos, Vector3 targetPos, float t)
    {

        float curX = Mathf.Lerp(startPos.x, targetPos.x, t);
        float curY = Mathf.Lerp(startPos.y, targetPos.y, t);
        return new Vector3(curX, curY, 1);
    }

    // 手卡->其他动画,移动世界坐标
    private IEnumerator MoveToCardPoolAnimation(Vector3 pos)
    {
        canBreakAnimation = false;
        Vector3 curPos = transform.position;
        float timer = 0;
        
        while (transform.position.x != pos.x || transform.position.y != pos.y)
        {
            transform.localScale = Vector3Lerp(new Vector3(1, 1, 1), new Vector3(0, 0, 0), timer / Time_Animation_Move);
            transform.position = Vector3Lerp(curPos, pos, timer / Time_Animation_Move);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canBreakAnimation = true;
        OnAnimationOver();
    }


    //-------------------------------------------- 卡牌动画结束 ---------------------------------------------------//
    //-------------------------------------------------------------------------------------------------------------//
    //-------------------------------------------- 卡牌信息开始 ---------------------------------------------------//

    public Text CardName_Text;
    public Text Description_Text;

    // 卡牌更换
    public void OnCardChanged(Card card)
    {
        CardName_Text.text = card.name;
        Description_Text.text = card.GetCardDescription();
    }
}
