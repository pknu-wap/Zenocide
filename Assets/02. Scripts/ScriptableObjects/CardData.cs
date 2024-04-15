// 김민철
using UnityEngine;

// 카드의 종류
public enum EffectType
{
    Attack,         // 공격
    Shield,         // 실드 생성
    Heal,           // 체력 회복
    Cleanse,        // 디버프 제거
    RestoreCost,    // 코스트 회복
    Draw,           // 카드 드로우
    Buff,           // 버프
    Debuff          // 버프
}

// 유니티 에디터, Project 뷰의 Create 메뉴에 아래 항목을 추가한다.
[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    [Header("카드 모양")]
    // 카드 이름
    public new string name;
    // 카드 설명
    [TextArea(3, 5)]
    public string description;
    // 카드 일러스트
    public Sprite sprite;

    [Header("카드 성능")]
    // 카드 발동에 필요한 코스트
    public int cost;
    // 카드 효과에 적용되는 수치
    public int amount;
    // 카드 효과의 종류
    public EffectType type;
}
