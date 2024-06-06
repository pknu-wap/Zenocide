// 김민철
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class BuffEffect
{
    public BuffEffect(SkillType type, int damagePerTurn, int remainingTurns)
    {
        this.type = type;
        this.damagePerTurn = damagePerTurn;
        this.remainingTurns = remainingTurns;
    }

    public SkillType type;
    public int damagePerTurn;
    public int remainingTurns;
}

public class buffIconComponent
{
    public buffIconComponent(Image image, TMP_Text tmp_Text)
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
    [SerializeField] protected int shield = 0;

    [Header("능력치")]
    // 추가 공격력. 데미지 계산 식에 적용
    public int bonusAttackStat = 0;
    // 추가 데미지. 데미지 계산 후, 추가로 들어가는 고정 데미지
    public int bonusDamage = 0;
    // 추가 방어력. 데미지 계산 식에 적용
    public int bonusArmor = 0;

    // 디버그용, 추후 삭제
    [Header("컴포넌트")]
    // 스프라이트
    protected Image imageComponent;
    // HP 바
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TMP_Text hpText;
    // 실드 바
    protected Image shieldBar;
    // 버프 아이콘 생성기 구현 예정 -> 오브젝트 풀링으로 대체
    protected Transform statusPanel;
    // 디버프창
    protected List<buffIconComponent> buffIcons;
    protected TMP_Text[] buffName;
    protected TMP_Text[] buffDescription;
    
    // 데미지 텍스트
    [SerializeField] protected GameObject damageTextPrefab;

    [Header("상태이상")]
    protected Transform buffIconContainer;
    protected List<BuffEffect> buffs;

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
        hpBar = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
        hpText = hpBar.transform.GetChild(0).GetComponent<TMP_Text>();

        // 실드 바
        shieldBar = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();

        // 디버프 효과들(내부 데이터)을 담아둘 리스트
        buffs = new List<BuffEffect>();
        buffIcons = new List<buffIconComponent>();

        // 디버프 아이콘들의 부모 컨테이너
        buffIconContainer = transform.GetChild(0).GetChild(1).GetChild(1);
        // debuffIconContainer의 모든 자식 오브젝트를 비활성화. (자식의 자식은 X)
        foreach (Transform icon in buffIconContainer)
        {
            // debuffIcons에 icon의 이미지, 텍스트 컴포넌트를 할당
            buffIcons.Add(new buffIconComponent(icon.GetComponent<Image>(), icon.GetChild(0).GetComponent<TMP_Text>()));
            // 그리고 비활성화.
            icon.gameObject.SetActive(false);
        }

        // 디버프 상세정보창을 불러온다. (더미, 행동정보창 제외)
        statusPanel = transform.GetChild(1).GetChild(0);
        buffName = new TMP_Text[statusPanel.childCount - 1];
        buffDescription = new TMP_Text[statusPanel.childCount - 1];

        for (int i = 1; i < statusPanel.childCount; ++i)
        {
            buffName[i - 1] = statusPanel.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            buffDescription[i - 1] = statusPanel.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
        }
    }

    protected virtual void StartBattle()
    {
        UpdateShieldUI();
        UpdateHPUI();

        CleanseDebuff();
        ResetStat();
    }

    #region HP
    // 이 오브젝트의 hp를 반환한다.
    public int GetCurrentHP()
    {
        return currentHp;
    }
    
    // 이 오브젝트의 최대 HP를 반환한다.
    public int GetMaxHP()
    {
        return maxHp;
    }

    // 현재 HP를 갱신한다.
    public void UpdateHPUI()
    {
        // 이미지 변경
        // HP 바를 뚫으려 하면
        if (currentHp + shield > maxHp)
        {
            // 비율을 맞춰 현재 체력 바의 크기를 줄이고
            hpBar.fillAmount = (float)currentHp / (maxHp + shield);
        }
        // 아니라면
        else
        {
            // 그대로 출력한다.
            hpBar.fillAmount = (float)currentHp / maxHp;
        }

        // 텍스트 변경
        // 실드가 없다면 현재 체력과 최대 체력을
        string text = currentHp + "/" + maxHp;
        // 그게 아니라면
        if(shield > 0)
        {
            // 방어막도 함께 적어준다.
            text = "(" + currentHp + " + " + shield + ") / " + maxHp;
        }
        hpText.text = text;
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void DecreaseHP(int damage)
    {
        // 현재 데미지
        int currentDamage = damage;

        // 실드가 있다면 데미지 재계산
        if (shield > 0)
        {
            // currentDamage를 감소시키고
            currentDamage -= shield;
            if (currentDamage < 0)
            {
                // 잔여 데미지가 음수면 0으로 적용한다.
                currentDamage = 0;
            }

            // 실드에선 기존 데미지를 뺀다.
            shield -= damage;
            if (shield < 0)
            {
                // 잔여 방어막이 음수면 0으로 적용한다.
                shield = 0;
            }
        }

        // hp를 잔여 데미지 만큼 감소시킨다.
        currentHp -= currentDamage;

        // UI를 갱신한다.
        UpdateShieldUI();
        UpdateHPUI();

        // 적이 피격될 때 모션, 데미지 텍스트 출력
        if(this != Player.Instance && currentDamage > 0)
        {
            DamageText damageText = Instantiate(damageTextPrefab, transform.GetChild(0)).GetComponent<DamageText>();
            StartCoroutine(damageText.PrintDamageText(currentDamage));
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
        UpdateShieldUI();
        UpdateHPUI();
    }
    #endregion HP

    #region 실드
    // 방어막을 얻는다.
    public void GetShield(int amount)
    {
        // 방어막을 추가하고
        shield += amount;

        // UI 변경
        UpdateShieldUI();
        UpdateHPUI();
    }

    // 실드 UI를 갱신한다.
    private void UpdateShieldUI()
    {
        // 실드가 없다면
        if(shield == 0)
        {
            // 0으로 만들고 종료
            shieldBar.fillAmount = 0;
            return;
        }

        // HP 바를 뚫으려 하면
        if(currentHp + shield > maxHp)
        {
            // 100으로 만들고
            shieldBar.fillAmount = 1;
        }

        // 그 외는
        else
        {
            // 이미지를 변경한다. Shield를 뒤에 깔고 HP를 앞으로 빼서, 아래 식이 쓰인다.
            shieldBar.fillAmount = (float)(currentHp + shield) / (maxHp);
        }
    }
    #endregion 실드

    public virtual void Die()
    {
        // 죽음과 관련된 효과 처리
        // 죽음 애니메이션
    }

    #region 버프
    // 버프 효과를 턴 시작 이벤트에 등록한다.
    public void EnrollBuff(BuffEffect bleedEffect)
    {
        // 출혈 리스트에 추가
        buffs.Add(bleedEffect);

        // 출혈 디버프 UI 추가
        int i = buffs.Count - 1;

        UpdateBuffIcon(i);
    }

    // 디버프를 전부 제거한다.
    public void CleanseDebuff()
    {
        buffs.Clear();

        UpdateAllBuffIcon();
    }

    public void UpdateBuffIcon(int index)
    {
        // i번째 아이콘와 숫자를 변경하고
        buffIcons[index].image.sprite = CardInfo.Instance.skillIcons[(int)buffs[index].type];
        buffIcons[index].tmp_Text.text = buffs[index].remainingTurns.ToString();

        // i번째 디버프창의 내용을 갱신한다
        buffName[index].text = DebuffInfo.debuffNameDict[buffs[index].type];
        // 이렇게 $와 {}를 쓰면 변수명과 문자열을 섞어쓸 수 있다.
        buffDescription[index].text = $"{buffs[index].remainingTurns}{DebuffInfo.debuffDescriptionDict[buffs[index].type]}";

        // 오브젝트 활성화
        // 이 구문들은 리팩토링이 필요해보인다.
        // 아이콘 컨테이너 열기
        buffIconContainer.GetChild(index).gameObject.SetActive(true);
        // 상세정보창 컨테이너 열기
        buffName[index].gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateAllBuffIcon()
    {
        // 모든 현재 디버프에 대해(i번째 디버프에 대해)
        int i = 0;
        for (; i < buffs.Count; ++i)
        {
            UpdateBuffIcon(i);
        }

        // 디버프가 없는 아이콘들은
        for (; i < buffIconContainer.childCount; ++i)
        {
            // 비활성화한다.
            buffIconContainer.GetChild(i).gameObject.SetActive(false);
            buffName[i].gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    // 출혈 효과를 발생시킨다.
    public void GetBleedAll()
    {
        // 받게 될 전체 데미지
        int totalDamage = 0;

        // 모든 적용 중인 출혈 효과에 대해
        for(int i = 0; i < buffs.Count; ++i)
        {
            totalDamage += buffs[i].damagePerTurn;

            // 남은 턴 1 감소
            --buffs[i].remainingTurns;
            // 남은 턴이 0 이하라면
            if (buffs[i].remainingTurns <= 0)
            {
                // 해당 출혈 효과를 삭제한다.
                buffs.RemoveAt(i);
                // 뒤의 디버프들이 1칸씩 앞으로 땡겨졌으니, 인덱스도 1 앞으로 조정
                --i;
            }
        }

        // 출혈 이펙트 재생

        // 체력을 감소시킨다.
        DecreaseHP(totalDamage);

        // 아이콘을 업데이트 한다.
        UpdateAllBuffIcon();
    }
    #endregion 디버프

    #region 스텟

    // 모든 능력치 값을 초기화한다.
    public void ResetStat()
    {
        bonusAttackStat = 0;
        bonusDamage = 0;
        bonusArmor = 0;
    }

    public void GetBonusAttackStat(int amount)
    {
        bonusAttackStat += amount;
    }

    public void GetBonusDamage(int amount)
    {
        bonusDamage += amount;
    }
    #endregion 스텟
}
