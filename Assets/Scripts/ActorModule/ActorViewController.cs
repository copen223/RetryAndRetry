using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorViewController : MonoBehaviour
{
    [Header("攻击动画插值曲线")]
    public AnimationCurve AttackAnimationCurve;
    public float AttackAnimation_Time;

    public bool IsAnimating;



    public enum ActorAnimationName
    {
        AttackLeft,
        AttackRight,
        AttackDown,
        AttackUp
    }

    public void StartAnimation(ActorAnimationName animationName)
    {
        IsAnimating = true;
        switch (animationName)
        {
            case ActorAnimationName.AttackDown:StartCoroutine(AttackAnimation(Vector3.down, 0.7f));break;
            case ActorAnimationName.AttackUp: StartCoroutine(AttackAnimation(Vector3.up, 0.7f)); break;
            case ActorAnimationName.AttackRight: StartCoroutine(AttackAnimation(Vector3.right, 1)); break;
            case ActorAnimationName.AttackLeft: StartCoroutine(AttackAnimation(Vector3.left, 1)); break;
        }
        
    }

    IEnumerator AttackAnimation(Vector3 dir,float dis)
    {
        dir = dir.normalized;
        float x = 0;
        float timer = 0;
        while(timer < AttackAnimation_Time)
        {
            float t = timer / AttackAnimation_Time;
            x = AttackAnimationCurve.Evaluate(t) * dis;
            var dirWithDis = dir * x;
            transform.localPosition = dirWithDis;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        OnAnimationOver();
    }

    void OnAnimationOver()
    {
        IsAnimating = false;
    }

}
