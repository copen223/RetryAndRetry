using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class BattleMessagesConsoler : MonoBehaviour
{
    [SerializeField]
    private Text MessagesText = null;
    [SerializeField]
    private Scrollbar TextScrollbar = null;

    public void ConsoleMessage(string message)
    {
        MessagesText.text += message + "\n";

        // 下面是为了让消息显示最新
        var results = UIManager.instance.UIsHitByMouse();

        foreach (var result in results)
        {
            if (result.gameObject == gameObject)
                return;
        }

        RepeatSetPosAsync(0.2f);
        //GetComponent<ScrollRect>().StopMovement();
    }

    async void RepeatSetPosAsync(float time)
    {
        float timer = 0;
        while(timer < time)
        {
            timer += Time.deltaTime;
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
            await Task.Yield();
        }

    }

    private void Update()
    {
    }
}
