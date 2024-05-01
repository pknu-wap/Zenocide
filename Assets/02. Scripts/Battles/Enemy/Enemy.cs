using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적의 HP(체력)
    protected int hp;
    protected int attack;

    // 이 오브젝트의 hp를 반환한다.
    public int GetHP()
    {
        return hp;
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void DecreaseHP(int damage)
    {
        // hp를 damage만큼 감소시킨다.
        hp -= damage;

        // hp가 0 이하가 될 경우
        if(hp <= 0)
        {
            // 죽음 이벤트 실행
            Die();
        }
    }

    public void Die()
    {
        // 죽음과 관련된 효과 처리
        // 죽음 애니메이션
        // 오브젝트 비활성화
        gameObject.SetActive(false);
        // Battle Info에 남은 적 -1
    }
}
