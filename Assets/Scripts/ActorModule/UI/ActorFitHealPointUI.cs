using ActorModule.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ActorModule.UI
{
    public class ActorFitHealPointUI : ActorFitUI
    {
        public Image HealPointImage;
        public Text HealPointText;

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
}
