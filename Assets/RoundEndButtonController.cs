using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundEndButtonController : MonoBehaviour
{
    private bool isUsable;
    public void SetUsable(bool usable)
    {
        isUsable = usable;
    }
    public void TurnEnd()
    {
        if(isUsable)
        BattleManager.instance.SendMessage("OnTurnEnd");
    }

    // 消息响应
    private void OnAnimationStart() { SetUsable(false); }
    private void OnAnimationOver() { SetUsable(true); }

}
