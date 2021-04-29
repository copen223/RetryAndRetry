using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "name", menuName = "MyInfo/牌组")]
public class DeckInfo : ScriptableObject
{
    public List<string> CardWithCount = new List<string>();
}
