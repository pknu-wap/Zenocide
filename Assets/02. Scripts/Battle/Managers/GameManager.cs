using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() => Instance = this;

    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] Transform enemiesParent;
    [SerializeField] private Enemy[] enemies;

    void Start()
    {
        StartGame();

        // Enemy 프리팹들을 미리 등록해둔다.
        enemies = enemiesParent.GetComponentsInChildren<Enemy>();
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.S) && TurnManager.Instance.myTurn)
            TurnManager.OnAddCard?.Invoke(true);
    }

    public void StartGame()
    {
        StartCoroutine(TurnManager.Instance.StartGameCo());
    }

    public void TestCreateEnemy()
    {
        EnrollEnemies(new string[] { "NormalZombie", "NormalZombie" });
    }

    public void EnrollEnemies(string[] enemyNames)
    {
        int i = 0;
        for(; i < enemyNames.Length; ++i)
        {
            enemies[i].UpdateEnemyData(EnemyInfo.Instance.GetEnemyData(enemyNames[i]));
            enemies[i].gameObject.SetActive(true);
        }

        for(; i < enemies.Length; ++i)
        {
            enemies[i].gameObject.SetActive(false);
        }
    }

    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }
}
