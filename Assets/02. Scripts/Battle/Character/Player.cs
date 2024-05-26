// 김민철

public class Player : Character
{
    #region 싱글톤
    public static Player Instance { get; set; }
    private static Player instance;

    public override void Awake()
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

    public void Start()
    {
        GameManager.Instance.onStartBattle.AddListener(StartBattle);

        TurnManager.Instance.onEndPlayerTurn.AddListener(EndPlayerTurn);
    }

    // 전투를 시작할 때 실행할 함수들
    protected override void StartBattle()
    {
        base.StartBattle();
    }

    public void EndPlayerTurn()
    {
        GetBleedAll();
    }

    public override void Die()
    {
        // 플레이어의 사망을 알림
        BattleInfo.Instance.isGameOver = true;

        currentHp = 0;
        UpdateCurrentHP();

        GameManager.Instance.Notification("게임 오버");
    }
}
