// 김민철
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    #region 싱글톤
    public static CardInfo instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        // 카드 효과를 델리게이트에 모두 등록
        EnrollAllEffect();

        // 사용 예시
        // CardInfo.Instance.effects[(int)효과 종류](효과량, 대상);
        // CardInfo.Instance.effects[(int)EffectType.Buff](5, gameObject);
    }
    #endregion 싱글톤

    #region 카드 효과
    // 카드 효과 함수들을 담아둘 델리게이트
    public delegate void CardEffects(int amount, GameObject target);
    // 델리게이트 배열, EffectType에 맞는 함수를 매칭한다.
    public CardEffects[] effects = new CardEffects[7];

    // 모든 효과를 effects 배열에 등록한다.
    void EnrollAllEffect()
    {
        // 카드 효과를 배열에 등록
        effects[(int)EffectType.Attack] += Attack;
        effects[(int)EffectType.Shield] += Shield;
        effects[(int)EffectType.Heal] += Heal;
        effects[(int)EffectType.Cleanse] += Cleanse;
        effects[(int)EffectType.RestoreCost] += RestoreCost;
        effects[(int)EffectType.Draw] += Draw;
        effects[(int)EffectType.Buff] += Buff;
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    public void Attack(int amount, GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();

        enemy.DecreaseHP(amount);
        Debug.Log(enemy.GetHP());
    }

    public void Shield(int amount, GameObject target)
    {
        Debug.Log("Shield");
    }

    public void Heal(int amount, GameObject target)
    {
        Debug.Log("Heal");
    }

    public void Cleanse(int amount, GameObject target)
    {
        Debug.Log("Cleanse");
    }

    public void RestoreCost(int amount, GameObject target)
    {
        Debug.Log("RestoreCost");
    }

    public void Draw(int amount, GameObject target)
    {
        Debug.Log("Draw");
    }

    public void Buff(int amount, GameObject target)
    {
        Debug.Log("Buff");
    }
    #endregion 카드 효과
}
