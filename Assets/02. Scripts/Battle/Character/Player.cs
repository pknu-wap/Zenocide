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
}
