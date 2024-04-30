// 김민철
using UnityEngine;
using System;

public class CardInfo : MonoBehaviour
{
    #region 싱글톤
    public static CardInfo Instance { get; set; }
    private static CardInfo instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        // 카드 효과를 델리게이트에 모두 등록
        EnrollAllEffects();
        EnrollTargetDict();
    }
    #endregion 싱글톤

    #region 카드 UI 데이터
    // 통합하고 싶다...
    public Sprite[] skillIcons;
    public Sprite[] debuffIcons;
    #endregion 카드 UI 데이터

    #region 정보 검색
    // 카드 타입을 넣으면 레이어를 뱉어주는 배열
    private LayerMask[] layerDict;

    // 배열에 타입 - 레이어 정보를 등록한다.
    private void EnrollTargetDict()
    {
        layerDict = new LayerMask[Enum.GetValues(typeof(EffectType)).Length];

        layerDict[(int)EffectType.Attack] = LayerMask.GetMask("Enemy");
        layerDict[(int)EffectType.Shield] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.Heal] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.Cleanse] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.RestoreCost] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.Draw] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.Buff] = LayerMask.GetMask("Field");
        layerDict[(int)EffectType.Debuff] = LayerMask.GetMask("Enemy");
        layerDict[(int)EffectType.Bleed] = LayerMask.GetMask("Enemy");
    }

    // 타입에 맞는 레이어를 반환한다.
    public LayerMask ReturnLayer(EffectType type)
    {
        return layerDict[(int)type];
    }
    #endregion 정보 검색

    #region 카드 효과
    // 카드 효과 함수들을 담아둘 델리게이트
    // amount는 카드의 효과량, turnCount는 지속될 턴의 수, target은 적용 대상이다.
    // 예를 들어 Bleed의 변수가 6, 3, Player.Instance라면, 플레이어에게 3턴간, 매 턴 6의 데미지를 준다.
    public delegate void CardEffects(int amount, int turnCount, Character target);
    // 델리게이트 배열, EffectType에 맞는 함수를 매칭한다.
    public CardEffects[] effects;

    // 사용 예시
    // CardInfo.Instance.effects[(int)효과 종류](효과량, 대상);
    // CardInfo.Instance.effects[(int)EffectType.Buff](5, target);

    // 모든 효과를 effects 배열에 등록한다.
    void EnrollAllEffects()
    {
        effects = new CardEffects[Enum.GetValues(typeof(EffectType)).Length];

        // 카드 효과를 배열에 등록
        effects[(int)EffectType.Attack] += Attack;
        effects[(int)EffectType.Shield] += Shield;
        effects[(int)EffectType.Heal] += Heal;
        effects[(int)EffectType.Cleanse] += Cleanse;
        effects[(int)EffectType.RestoreCost] += RestoreCost;
        effects[(int)EffectType.Draw] += Draw;
        effects[(int)EffectType.Buff] += Buff;
        effects[(int)EffectType.Debuff] += Debuff;
        effects[(int)EffectType.Bleed] += Bleed;
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    public void Attack(int amount, int turnCount, Character target)
    {
        target.DecreaseHP(amount);
    }

    public void Shield(int amount, int turnCount, Character target)
    {
        Debug.Log("Shield");
    }

    public void Heal(int amount, int turnCount, Character target)
    {
        Debug.Log("Heal");
    }

    public void Cleanse(int amount, int turnCount, Character target)
    {
        Debug.Log("Cleanse");
    }

    public void RestoreCost(int amount, int turnCount, Character target)
    {
        Debug.Log("RestoreCost");
    }

    public void Draw(int amount, int turnCount, Character target)
    {
        Debug.Log("Draw");
    }

    public void Buff(int amount, int turnCount, Character target)
    {
        Debug.Log("Buff");
    }

    public void Debuff(int amount, int turnCount, Character target)
    {
        Debug.Log("Debuff");
    }

    public void Bleed(int amount, int turnCount, Character target)
    {
        target.EnrollBleed(new BleedEffect(amount, turnCount));
    }
    #endregion 카드 효과
}
