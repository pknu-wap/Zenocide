// 김민철
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

    public override void Die()
    {
        // 플레이어의 사망을 알림
        BattleInfo.Instance.isGameOver = true;

        // 깔끔하게 보이기 위한 클램핑
        currentHp = 0;
        UpdateHPUI();

        StartCoroutine(GameManager.Instance.GameOver());
    }
}
