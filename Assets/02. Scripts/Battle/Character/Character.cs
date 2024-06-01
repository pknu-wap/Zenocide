// 김민철
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class BleedEffect
{
    public BleedEffect(SkillType type, int damagePerTurn, int remainingTurns)
    {
        this.type = type;
        this.damagePerTurn = damagePerTurn;
        this.remainingTurns = remainingTurns;
    }

    public SkillType type;
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
    protected int currentHp = 100;
    [SerializeField] protected int maxHp = 100;

    // 디버그용, 추후 삭제
    [Header("컴포넌트")]
    // 스프라이트
    protected Image imageComponent;
    // HP 바
    protected Image hpBar;
    protected TMP_Text hpText;
    // 버프 아이콘 생성기 구현 예정 -> 오브젝트 풀링으로 대체
    protected Transform statusPanel;
    // 디버프창
    protected List<DebuffIconComponent> debuffIcons;
    protected TMP_Text[] debuffName;
    protected TMP_Text[] debuffDescription;

    [Header("상태이상")]
    protected Transform debuffIconContainer;
    protected List<BleedEffect> debuffs;

    [Header("이벤트")]
    protected UnityEvent onTurnStarted;

    public virtual void Awake()
    {

    }

    // 컴포넌트들을 등록한다.
    protected virtual void EnrollComponents()
    {
        // 스프라이트
        imageComponent = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        // HP 바
        hpBar = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        hpText = hpBar.transform.GetChild(0).GetComponent<TMP_Text>();

        // 디버프 효과들(내부 데이터)을 담아둘 리스트
        debuffs = new List<BleedEffect>();
        debuffIcons = new List<DebuffIconComponent>();

        // 디버프 아이콘들의 부모 컨테이너
        debuffIconContainer = transform.GetChild(0).GetChild(1).GetChild(1);
        // debuffIconContainer의 모든 자식 오브젝트를 비활성화. (자식의 자식은 X)
        foreach (Transform icon in debuffIconContainer)
        {
            // debuffIcons에 icon의 이미지, 텍스트 컴포넌트를 할당
            debuffIcons.Add(new DebuffIconComponent(icon.GetComponent<Image>(), icon.GetChild(0).GetComponent<TMP_Text>()));
            // 그리고 비활성화.
            icon.gameObject.SetActive(false);
        }

        // 디버프 상세정보창을 불러온다. (더미, 행동정보창 제외)
        statusPanel = transform.GetChild(1).GetChild(0);
        debuffName = new TMP_Text[statusPanel.childCount - 1];
        debuffDescription = new TMP_Text[statusPanel.childCount - 1];

        for (int i = 1; i < statusPanel.childCount; ++i)
        {
            debuffName[i - 1] = statusPanel.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            debuffDescription[i - 1] = statusPanel.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
        }
    }

    protected virtual void StartBattle()
    {
        UpdateCurrentHP();

        CleanseDebuff();
    }

    // 이 오브젝트의 hp를 반환한다.
    public int GetHP()
    {
        return currentHp;
    }

    // 현재 HP를 갱신한다.
    public void UpdateCurrentHP()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
        hpText.text = currentHp + "/" + maxHp;
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void DecreaseHP(int damage)
    {
        // hp를 damage만큼 감소시킨다.
        currentHp = Math.Max(currentHp - damage, 0);
        
        UpdateCurrentHP();

        // 적이 피격될 때 모션 출력
        if(this != Player.Instance && damage > 0)
        {
            imageComponent.transform.DOShakePosition(0.5f, 10f);
        }

        // hp가 0 이하가 될 경우
        if (currentHp <= 0)
        {
            // 죽음 이벤트 실행
            Die();
        }
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void IncreaseHP(int heal)
    {
        // hp를 damage만큼 감소시킨다.
        currentHp += heal;

        // hp가 최대치 이상이 될 경우
        if (currentHp > maxHp)
        {
            // 최대치로 맞추기
            currentHp = maxHp;
        }

        // UI 갱신
        UpdateCurrentHP();
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

        UpdateDebuffIcon(i);
    }

    // 디버프를 전부 제거한다.
    public void CleanseDebuff()
    {
        debuffs.Clear();

        UpdateAllDebuffIcon();
    }

    public void UpdateDebuffIcon(int index)
    {
        /*
         * 대입하는 값이 현재는 출혈로 고정되어 있다. (그거 밖에 안 그려서...)
         */
        // i번째 아이콘와 숫자를 변경하고
        debuffIcons[index].image.sprite = CardInfo.Instance.debuffIcons[0];
        debuffIcons[index].tmp_Text.text = debuffs[index].remainingTurns.ToString();

        // i번째 디버프창의 내용을 갱신한다
        debuffName[index].text = DebuffInfo.debuffNameDict[debuffs[index].type];
        // 이렇게 $와 {}를 쓰면 변수명과 문자열을 섞어쓸 수 있다.
        debuffDescription[index].text = $"{debuffs[index].remainingTurns}{DebuffInfo.debuffDescriptionDict[debuffs[index].type]}";

        // 오브젝트 활성화
        // 이 구문들은 리팩토링이 필요해보인다.
        debuffIconContainer.GetChild(index).gameObject.SetActive(true);
        debuffName[index].gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateAllDebuffIcon()
    {
        // 모든 현재 디버프에 대해(i번째 디버프에 대해)
        int i = 0;
        for (; i < debuffs.Count; ++i)
        {
            UpdateDebuffIcon(i);
        }

        // 디버프가 없는 아이콘들은
        for (; i < debuffIconContainer.childCount; ++i)
        {
            // 비활성화한다.
            debuffIconContainer.GetChild(i).gameObject.SetActive(false);
            debuffName[i].gameObject.transform.parent.gameObject.SetActive(false);
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

        // 아이콘을 업데이트 한다.
        UpdateAllDebuffIcon();
    }
}
