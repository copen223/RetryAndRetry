using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    [SerializeField]
    private Text timeText = null;

    [SerializeField]
    public Text description = null;

    /// <summary>
    /// 歪脖根据buff储存的sprite进行修改
    /// </summary>
    public Image buffImage = null;

    [SerializeField]
    private List<Sprite> buffSprites = new List<Sprite>();
    
    /// <summary>
    /// 修改UI的时间显示
    /// </summary>
    public int Time
    {
        set
        {
            if (value < 0)
                timeText.text = "";
            else
                timeText.text = value + "";
        }
    }
    /// <summary>
    /// 修改buff UI的图片类型，一般不用
    /// </summary>
    public BuffType BuffImageType
    {
        set
        {
            switch (value)
            {
                case BuffType.Attack: buffImage.sprite = buffSprites[0]; break;
                case BuffType.Dfense: buffImage.sprite = buffSprites[1]; break;
                case BuffType.Dodge: buffImage.sprite = buffSprites[2]; break;
                case BuffType.Hit: buffImage.sprite = buffSprites[3]; break;
            }

        }
    }

    public enum BuffType
    {
        Attack,
        Dfense,
        Dodge,
        Hit
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
