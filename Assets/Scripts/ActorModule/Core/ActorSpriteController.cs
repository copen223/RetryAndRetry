using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ActorSpriteController : MonoBehaviour
{
    public event Action MouseEnterEvent;
    public event Action MouseExitEvent;
    public void OnMouseExit()
    {
        MouseExitEvent?.Invoke();
    }

    private void OnMouseEnter()
    {
        MouseEnterEvent?.Invoke();
    }
}
