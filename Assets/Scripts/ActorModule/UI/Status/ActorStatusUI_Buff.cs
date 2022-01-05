using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStatusUI_Buff : MonoBehaviour
{
    [SerializeField]
    private GameObject buffUIPrefab = null;

    [SerializeField]
    private Transform buffUIParent = null;

    private List<GameObject> buffUIs = new List<GameObject>();

    public void UpdateThisUI(BuffController buffCon)
    {
        ClearBuffUIs();
        foreach (var buff in buffCon.buffs)
        {
            var go = Instantiate(buffUIPrefab,buffUIParent);
            buffUIs.Add(go);

            var buffUI = go.GetComponent<BuffUI>();

            if (buff.DurationType == Assets.Scripts.BufferModule.BuffDurationType.Turn)
                buffUI.Time = buff.DurationTime;
            else
                buffUI.Time = -1;

            buffUI.buffImage.sprite = buff.buffUIImage;
            buffUI.description.text = buff.GetDescription();
        }
    }

    private void ClearBuffUIs()
    {
        while(buffUIs.Count>=1)
        {
            Destroy(buffUIs[0]);
            buffUIs.RemoveAt(0);
        }
    }
}
