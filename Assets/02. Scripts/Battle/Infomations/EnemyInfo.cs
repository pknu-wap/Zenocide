using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public static EnemyInfo Instance { get; private set; }

    [Header("적 리스트")]
    // 코드에서 접근하는, enemyID를 받아 EnemyData를 반환하는 딕셔너리
    public Dictionary<string, EnemyData> enemyList;
    // 개발자가 등록하는 EnemyData의 종류
    public EnemyData[] enemies;

    private void Awake()
    {
        Instance = this;

        CreateEnemyDictionary();
    }

    private void CreateEnemyDictionary()
    {
        enemyList = new Dictionary<string, EnemyData>();

        foreach (EnemyData data in enemies)
        {
            enemyList.Add(data.enemyID, data);
        }
    }

    public EnemyData GetEnemyData(string enemyID)
    {
        return enemyList[enemyID];
    }
}
