using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("외관")]
    // 적의 이름, 식별자로 사용된다.
    public string enemyID;
    // 적의 일러스트
    public Sprite illust;

    [Header("내부 데이터")]
    // 적의 최대 HP
    public int maxHp;
    // 적이 사용 가능한 스킬 정보
    public Skill[] skills;
}
