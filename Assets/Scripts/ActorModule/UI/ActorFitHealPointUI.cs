using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorFitHealPointUI : MonoBehaviour
{
    [Header("血条相对位移")]
    public Vector3 HealPointOffset;

    public Image HealPointImage;
    public Text HealPointText;

    void Start()
    {
        //UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePosition()
    {
        transform.localPosition = HealPointOffset;
    }

    public void OnHealPointChanged(GameObject gb)
    {
        var actor = gb.GetComponent<ActorController>();
        float hp = actor.HealPoint;
        float max = actor.HealPoint_Max;
        float scale = hp / max;

        HealPointImage.transform.localScale = new Vector3(scale, 1, 1);
        HealPointText.text = hp + "/" + max;

    }
}
