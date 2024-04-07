// 김민철
using UnityEngine;

public enum EffectType
{
    Attack,         // 공격
    Shield,         // 실드 생성
    Heal,           // 체력 회복
    Cleanse,        // 디버프 제거
    RestoreCost,    // 코스트 회복
    Draw,           // 카드 드로우
    Buff            // 버프
}

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    [Header("카드 모양")]
    public new string name;
    [TextArea(3, 5)]
    public string description;
    public Sprite sprite;

    [Header("카드 성능")]
    public int cost;
    public int amount;
    public EffectType effect;
}
