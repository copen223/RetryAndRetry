using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ActionTip
{
    public class ActionTipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI UIText;
        private string tipText;
        
        public void Init(string text)
        {
            tipText = text;
            UIText.text = text;
        }

        public void SetActive(bool ifActive)
        {
            gameObject.SetActive(ifActive);
        }
        
       
    }
}
