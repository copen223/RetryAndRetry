using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorFitFocusTrailUI : ActorFitUI
{
    public Text FocusTrailCountText;

    public void OnFocusTrailCountChanged(GameObject gb)
    {
        var actor = gb.GetComponent<ActorController>();
        int count = actor.FocusTrailCount;
        FocusTrailCountText.text = count+"";
    }
}
