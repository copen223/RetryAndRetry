using System.Collections.Generic;
using UnityEngine;

namespace Resources.Infos
{
    [CreateAssetMenu(fileName = "name", menuName = "MyInfo/牌组")]
    public class DeckInfo : ScriptableObject
    {
        public List<string> CardWithCount = new List<string>();
    }
}
