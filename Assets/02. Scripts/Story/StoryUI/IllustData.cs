using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "IllustData", menuName = "Scriptable Objects/IllustData", order = 1)]
public class IllustData: ScriptableObject
{
    [Header("일러스트 데이터")]
    public Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1},
        {"선택지",2}
    };
}