// 김민철
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class BleedEffect
{
    public BleedEffect(int damagePerTurn, int remainingTurns)
    {
        this.damagePerTurn = damagePerTurn;
        this.remainingTurns = remainingTurns;
    }

    public int damagePerTurn;
    public int remainingTurns;
}

public class DebuffIconComponent
{
    public DebuffIconComponent(Image image, TMP_Text tmp_Text)
    {
        this.image = image;
        this.tmp_Text = tmp_Text;
    }

    public Image image;
    public TMP_Text tmp_Text;
}

public class Character : MonoBehaviour
{
    [Header("데이터")]
    // HP(체력)
    [SerializeField] protected int currentHp = 100;
    [SerializeField] protected int maxHp = 100;

    // 디버그용, 추후 삭제
    [Header("컴포넌트")]
    // HP 바
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TMP_Text hpText;
    //[SerializeField] protected BuffIconSpawner buffer;
    // 버프 아이콘 생성기 구현 예정 -> 오브젝트 풀링으로 대체

    [Header("상태이상")]
    [SerializeField] public List<BleedEffect> debuffs;
    [SerializeField] public List<DebuffIconComponent> debuffIcons;

    [Header("이벤트")]
    [SerializeField] public UnityEvent onTurnStarted;

    public virtual void Awake()
    {
        // HP 바
        hpBar = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        hpText = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();

        // 디버프 효과들(내부 데이터)을 담아둘 리스트
        debuffs = new List<BleedEffect>();
        debuffIcons = new List<DebuffIconComponent>();

        // 디버프 아이콘들의 부모 컨테이너
        debuffIconContainer = transform.GetChild(1).GetChild(0).GetChild(1);
        // debuffIconContainer의 모든 자식 오브젝트를 비활성화. (자식의 자식은 X)
        foreach (Transform icon in debuffIconContainer)
        {
            // debuffIcons에 icon의 이미지, 텍스트 컴포넌트를 할당
            debuffIcons.Add(new DebuffIconComponent(icon.GetComponent<Image>(), icon.GetChild(0).GetComponent<TMP_Text>()));
            // 그리고 비활성화.
            icon.gameObject.SetActive(false);
        }

        UpdateCurrentHP();
    }

    // 이 오브젝트의 hp를 반환한다.
    public int GetHP()
    {
        return currentHp;
    }

    public void UpdateCurrentHP()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
        hpText.text = currentHp + "/" + maxHp;
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void DecreaseHP(int damage)
    {
        // hp를 damage만큼 감소시킨다.
        currentHp -= damage;

        UpdateCurrentHP();

        // hp가 0 이하가 될 경우
        if (currentHp <= 0)
        {
            // 죽음 이벤트 실행
            Die();
        }
    }

    public virtual void Die()
    {
        // 죽음과 관련된 효과 처리
        // 죽음 애니메이션
    }

    // 출혈 효과를 턴 시작 이벤트에 등록한다.
    public void EnrollBleed(BleedEffect bleedEffect)
    {
        // 출혈 리스트에 추가
        debuffs.Add(bleedEffect);

        // 출혈 디버프 UI 추가
        int i = debuffs.Count - 1;
        Transform icon = debuffIconContainer.GetChild(i);
        /*
         * 인덱스 수정 필요
         */
        debuffIcons[i].image.sprite = CardInfo.Instance.debuffIcons[0];
        debuffIcons[i].tmp_Text.text = bleedEffect.remainingTurns.ToString();

        icon.gameObject.SetActive(true);
    }

    [Header("버프 아이콘")]
    public Transform debuffIconContainer;

    public void UpdateDebuffIcon()
    {
        // 모든 현재 디버프에 대해(i번째 디버프에 대해)
        int i = 0;
        for (; i < debuffs.Count; ++i)
        {
            // i번째 이미지와 텍스트를 변경하고
            debuffIcons[i].image.sprite = CardInfo.Instance.debuffIcons[0];
            debuffIcons[i].tmp_Text.text = debuffs[i].remainingTurns.ToString();
            
            // 오브젝트 활성화
            // 이 구문은 리팩토링이 필요해보인다.
            debuffIconContainer.GetChild(i).gameObject.SetActive(true);
        }

        // 디버프가 없는 아이콘들은
        for(;i < debuffIconContainer.childCount; ++i)
        {
            // 비활성화한다.
            debuffIconContainer.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 출혈 효과를 발생시킨다.
    public void GetBleedAll()
    {
        // 받게 될 전체 데미지
        int totalDamage = 0;

        // 모든 적용 중인 출혈 효과에 대해
        for(int i = 0; i < debuffs.Count; ++i)
        {
            totalDamage += debuffs[i].damagePerTurn;

            // 남은 턴 1 감소
            --debuffs[i].remainingTurns;
            // 남은 턴이 0 이하라면
            if (debuffs[i].remainingTurns <= 0)
            {
                // 해당 출혈 효과를 삭제한다.
                debuffs.RemoveAt(i);
                // 뒤의 디버프들이 1칸씩 앞으로 땡겨졌으니, 인덱스도 1 앞으로 조정
                --i;
            }
        }

        // 출혈 이펙트 재생

        // 체력을 감소시킨다.
        DecreaseHP(totalDamage);
        Debug.Log(totalDamage);

        // 아이콘을 업데이트 한다.
        UpdateDebuffIcon();
    }
}
