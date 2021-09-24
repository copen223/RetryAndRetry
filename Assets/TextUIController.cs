using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUIController : MonoBehaviour
{
    // 改变该值以改变文本值
    public int TextValue { set { ChangeValue(value); } }

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeValue(int val)
    {
        text.text = val + "";
    }

    public void ChangeText(string txt,Color color)
    {
        text.text = txt; text.color = color;
    }
}
