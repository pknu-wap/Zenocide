// 김민철
using DG.Tweening;
using UnityEngine;

public class Player : Character
{
    #region 싱글톤
    public static Player Instance { get; set; }

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

        EnrollComponents();
    }
    #endregion 싱글톤

    public string job;

    public void Start()
    {
        // 배틀을 시작하면 상태를 초기화한다.
        TurnManager.Instance.onEndPlayerTurn.AddListener(EndPlayerTurn);
    }

    public void EndPlayerTurn()
    {
        GetBuffAll();
        GetSilence();
    }

    public override void DecreaseHP(int damage)
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

        // 카메라 셰이크
        if(currentDamage > 0)
        {
            CamShake.Instance.Shake(0.2f, 0.7f * currentDamage, CamShake.Scene.Battle);
        }

        // hp가 0 이하가 될 경우
        if (currentHp <= 0)
        {
            currentHp = 0;
            UpdateHPUI();

            // 죽음 이벤트 실행
            Die();
        }
    }

    public override void Die()
    {
        // 플레이어의 사망을 알림
        BattleInfo.Instance.isGameOver = true;

        // 깔끔하게 보이기 위한 클램핑
        currentHp = 0;
        UpdateHPUI();

        StartCoroutine(GameManager.Instance.GameOver());
    }

    public void SavePlayerData()
    {
        DataManager.Instance.data.Hp = currentHp;
        DataManager.Instance.data.Job = job;
    }

    public void LoadHp()
    {
        currentHp = DataManager.Instance.data.Hp;
        UpdateHPUI();

        job = DataManager.Instance.data.Job;
    }

    override public void GetSilence()
    {
        // 침묵 스택
        int idxStack = GetBuffIndex(SkillType.SilenceStack);
        // 침묵 디버프
        int idxSilence = GetBuffIndex(SkillType.Silence);

        // 침묵 스택이 없다면 return
        if (idxStack == -1)
        {
            return;
        }

        int stack = buffs[idxStack].remainingTurns;

        // 침묵 조건이 충족되지 않으면 return
        if (buffs[idxStack].remainingTurns < 2)
        {
            return;
        }

        // 침묵이 걸려있지 않다면
        if (idxSilence == -1)
        {
            // 스택만큼 침묵을 적용
            EnrollBuff(new BuffEffect(SkillType.Silence, 0, stack / 2, this, this));
            // 스택을 차감
            ModifyBuff(idxStack, 0, -(stack - stack % 2));
        }
        else
        // 침묵이 걸려있다면
        {
            // 스택만큼 침묵을 추가
            ModifyBuff(idxSilence, 0, stack / 2);
            // 스택을 차감
            ModifyBuff(idxStack, 0, -(stack - stack % 2));
        }

        // 남은 스택이 0 이하라면
        if (buffs[idxStack].remainingTurns <= 0)
        {
            // 효과를 삭제한다.
            buffs.RemoveAt(idxStack);

            // 오브젝트 풀의 자원들을 반환한다.
            buffIcons[idxStack].ReleaseObject();
            buffIcons.RemoveAt(idxStack);
            buffInfoPanels[idxStack].ReleaseObject();
            buffInfoPanels.RemoveAt(idxStack);
        }

        // 아이콘 최신화
        UpdateAllBuffIcon();
    }
}
