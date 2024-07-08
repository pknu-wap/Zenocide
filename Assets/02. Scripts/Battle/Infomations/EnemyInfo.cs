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

    // enemyId를 키로, EnemyData를 밸류로 갖는 Dictionary를 생성한다.
    private void CreateEnemyDictionary()
    {
        enemyList = new Dictionary<string, EnemyData>();

        for(int i = 0; i <  enemies.Length; i++)
        {
                enemyList.Add(enemies[i].enemyId, enemies[i]);
        }
    }

    // enemyId에 맞는 EnemyData를 반환한다.
    public EnemyData GetEnemyData(string enemyId)
    {
        if (enemyList.ContainsKey(enemyId) == false)
        {
            return null;
        }

        return enemyList[enemyId];
    }
}
