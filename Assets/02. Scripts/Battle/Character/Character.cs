// 김민철
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffEffect
{
    public BuffEffect(SkillType type, int amount, int remainingTurns, Character target, Character caller)
    {
        this.type = type;
        this.amount = amount;
        this.remainingTurns = remainingTurns;
        this.target = target;
        this.caller = caller;
    }

    public SkillType type;
    public int amount;
    public int remainingTurns;
    public Character target;
    public Character caller;
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
    // 추가 출혈 데미지
    public int bonusBleedDamage = 0;

    [Header("일러스트")]
    // 스프라이트
    protected Image imageComponent;

    [Header("HP 바")]
    // HP 바
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TMP_Text hpText;
    // 실드 바
    [SerializeField] protected Image shieldBar;
    
    // 데미지 텍스트
    [SerializeField] protected GameObject damageTextPrefab;

    [Header("상태이상")]
    [SerializeField] protected List<BuffEffect> buffs;
    [SerializeField] protected List<BuffIcon> buffIcons;
    [SerializeField] protected List<BuffInfoPanel> buffInfoPanels;

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
        buffIcons = new List<BuffIcon>();
        buffInfoPanels = new List<BuffInfoPanel>();
    }

    public virtual void ResetState()
    {
        // 스탯을 초기화한다.
        ResetStat();

        // 체력 데이터 및 UI를 초기화한다.
        shield = 0;
        UpdateHPUI();
        UpdateShieldUI();

        // 버프를 제거한다.
        CleanseDebuff();
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
    public virtual void DecreaseHP(int damage)
    {
        
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
    protected void UpdateShieldUI()
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
    public void EnrollBuff(BuffEffect buff)
    {
        // 버프 리스트에 추가
        buffs.Add(buff);

        // BuffIcon과 BuffInfoPanel을 풀에서 가져와서
        BuffIcon buffIcon = ObjectPoolManager.Instance.GetGo("BuffIcon_"+this.name).GetComponent<BuffIcon>();
        BuffInfoPanel BuffInfoPanel = ObjectPoolManager.Instance.GetGo("BuffInfoPanel_"+this.name).GetComponent<BuffInfoPanel>();

        buffIcon.transform.SetAsLastSibling();
        BuffInfoPanel.transform.SetAsLastSibling();

        // 리스트에 등록한다.
        buffIcons.Add(buffIcon);
        buffInfoPanels.Add(BuffInfoPanel);

        // 버프 UI 갱신
        UpdateBuffIcon(buffs.Count - 1);
    }

    // 디버프를 전부 제거한다.
    public void CleanseDebuff()
    {
        buffs.Clear();

        for(int i = 0; i < buffIcons.Count; ++i)
        {
            buffIcons[i].ReleaseObject();
            buffInfoPanels[i].ReleaseObject();
        }

        buffIcons.Clear();
        buffInfoPanels.Clear();

        UpdateAllBuffIcon();
    }

    public void UpdateBuffIcon(int index)
    {
        if (buffs[index].remainingTurns <= 0)
        {
            return;
        }

        // 아이콘과 숫자를 변경한다.
        buffIcons[index].SetContent(CardInfo.Instance.skillIcons[(int)buffs[index].type], (buffs[index].remainingTurns).ToString());

        // 스킬 텍스트를 만든다.
        SkillText buffText = DebuffInfo.GetSkillText(buffs[index]);
        // 이름과 설명을 변경한다.
        buffInfoPanels[index].SetContent(buffText.name, buffText.description);
    }

    public void UpdateAllBuffIcon()
    {
        // 모든 현재 디버프에 대해(i번째 디버프에 대해)
        for (int i = 0; i < buffs.Count; ++i)
        {
            UpdateBuffIcon(i);
        }
    }

    public void GetBuffAll()
    {
        // 스탯 초기화
        //ResetStat();

        // 모든 적용 중인 버프에 대해
        for (int i = 0; i < buffs.Count; ++i)
        {
            // 스킬 발동
            CardInfo.Instance.ActivateSkill(buffs[i], this, buffs[i].caller);

            // 남은 턴 1 감소
            if (buffs[i].type != SkillType.SilenceStack)
            {
                --buffs[i].remainingTurns;
            }

            // 남은 턴이 0 이하라면
            if (buffs[i].remainingTurns <= 0)
            {
                // 해당 효과를 삭제한다.
                buffs.RemoveAt(i);

                // 오브젝트 풀의 자원들을 반환한다.
                buffIcons[i].ReleaseObject();
                buffIcons.RemoveAt(i);
                buffInfoPanels[i].ReleaseObject();
                buffInfoPanels.RemoveAt(i);

                // 뒤의 디버프들이 1칸씩 앞으로 땡겨졌으니, 인덱스도 1 앞으로 조정
                --i;
            }

            // 추가 데미지 텍스트를 갱신한다.
            CardManager.Instance.SetExtraDamage();

            // 이펙트 재생

            // 0.1초 딜레이
            // yield return new WaitForSeconds(0.1f);
        }
        
        // 아이콘 최신화
        UpdateAllBuffIcon();
    }

    // buffs의 매개변수에 해당하는 첫 버프의 인덱스를 반환한다
    // 해당 타입의 버프가 없다면 -1을 반환한다
    public int GetBuffIndex(SkillType type)
    {
        for(int i = 0;i < buffs.Count; ++i)
        {
            if (buffs[i].type == type)
            {
                return i;
            }
        }

        return -1;
    }

    // 버프의 amount와 remainingTurns를 수정
    public void ModifyBuff(int idx, int amount, int turnCount)
    {
        buffs[idx].amount += amount;
        buffs[idx].remainingTurns += turnCount;
    }

    virtual public void GetSilence()
    {

    }
    #endregion 디버프

    #region 스텟
    public void ResetStat()
    {
        bonusAttackStat = 0;
        bonusDamage = 0;
        bonusArmor = 0;
        bonusBleedDamage = 0;
    }

    public void GetBonusAttackStat(int amount)
    {
        bonusAttackStat += amount;
    }

    public void GetBonusDamage(int amount)
    {
        bonusDamage += amount;
    }

    public void GetBonusBleedDamage(int amount)
    {
        bonusBleedDamage += amount;
    }
    #endregion 스텟
}
