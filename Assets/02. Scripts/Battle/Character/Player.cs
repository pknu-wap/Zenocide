// 김민철

public class Player : Character
{
    #region 싱글톤
    public static Player Instance { get; set; }
    private static Player instance;

    public override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion 싱글톤

    protected override void Start()
    {
        base.Start();

        TurnManager.Instance.onEndPlayerTurn.AddListener(EndPlayerTurn);
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
