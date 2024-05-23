using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "IllustData", menuName = "Scriptable Objects/IllustData", order = 1)]
public class IllustData: ScriptableObject
{
    [Header("Text 데이터")]
    // 대화창 등장인물 이미지 테이블
    public readonly Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1},
        {"선택지",2}
    };
}