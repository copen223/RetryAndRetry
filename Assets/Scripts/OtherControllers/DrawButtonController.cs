using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawButtonController : MonoBehaviour
{
    public GameObject Hand;
    public void Draw()
    {
        Hand.SendMessage("DrawCard");
    }
}
