using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BufferModule;
using System.Linq;

public class BuffUIController : MonoBehaviour
{
    [SerializeField]
    private Transform buffUIsParent = null;

    [SerializeField]
    private GameObject buffUIPrefab = null;

    [SerializeField]
    private List<BuffUI> buffUIs = new List<BuffUI>();
    public void UpdateBuffUIs(BuffController buffcon)
    {
        List<Buff> buffs = new List<Buff>(
                           from buff in buffcon.buffs
                           where buff.IfAcitve
                           select buff
                           );

        for(int i = 0;i<buffs.Count;i++)
        {
            if( i > buffUIs.Count - 1)
            {
                buffUIs.Add(GameObject.Instantiate(buffUIPrefab, buffUIsParent).GetComponent<BuffUI>());
            }

            if (buffs[i].DurationType == BuffDurationType.Turn)
                buffUIs[i].Time = buffs[i].DurationTime;
            else
                buffUIs[i].Time = -1;

            buffUIs[i].buffImage.sprite = buffs[i].buffUIImage;

        }

        for(int i= buffs.Count;i < buffUIs.Count;)
        {
            Destroy(buffUIs[i].gameObject);
            buffUIs.RemoveAt(i);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
