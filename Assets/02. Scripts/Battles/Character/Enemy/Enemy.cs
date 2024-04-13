// 김민철
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // 적이 사용할 스킬 리스트
    public GameObject[] skills;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DecreaseHP(6);
        }
    }

    public override void Die()
    {
        base.Die();

        // 오브젝트 비활성화
        gameObject.SetActive(false);
        // Battle Info에 남은 적 -1
    }
}
