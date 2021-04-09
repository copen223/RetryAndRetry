using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : MonoBehaviour
{
    public BattleManager Manager;
    public virtual void StateStart() { }

    public virtual void StateUpdate() { }

    public virtual void StateExit() { }

    public void ChangeStateTo<T>()
    {
        foreach (BattleState battleState in Manager.BattleStates)
        {
            if (battleState is T)
            {
                Manager.currentState.StateExit();
                Manager.currentState = battleState;
                Manager.currentState.StateStart();
            }
        }
    }

    public void ChangeStateTo<T>(System.Action onStateChange)
    {
        foreach (BattleState battleState in Manager.BattleStates)
        {
            if (battleState is T)
            {
                Manager.currentState.StateExit();
                onStateChange?.Invoke();
                Manager.currentState = battleState;
                Manager.currentState.StateStart();
            }
        }
    }

    public bool IsEventProtecting;

    public void SetEventProtect()
    {
        IsEventProtecting = true;
        StartCoroutine("DelayStopEventProtect");
    }

    IEnumerator DelayStopEventProtect()
    {
        yield return new WaitForSeconds(0.1f);
        IsEventProtecting = false;
    }

}
