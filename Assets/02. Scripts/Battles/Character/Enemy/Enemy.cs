// 김민철
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [Header("정보")]
    // 적이 사용할 스킬 리스트
    public SkillData skillData;

    [Header("컴포넌트")]
    public Image behaviorIcon;
    public TMP_Text behaviorAmount;

    public override void Awake()
    {
        base.Awake();

        behaviorIcon = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
        behaviorAmount = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DecreaseHP(6);
        }
    }

    [Header("런타임 변수")]
    // 현재 준비 중인 스킬
    public Skill currentSkill;

    // 스킬 사용을 준비한다.
    public void ReadySkill()
    {
        // 1. 랜덤한 스킬들 중 하나를 선택한다.
        currentSkill = skillData.skills[0];

        // 2.UI를 갱신한다.
        // 2-1. 자신이 고른 스킬을 체력바 위에 표시한다.
        // 2-2. 상세정보창을 스킬의 설명으로 갱신한다.
    }

    // 스킬을 사용한다.
    public void CastSkill()
    {
        GameObject target;

        // 공격 스킬이면 Player를, 그 외는 자신을 타겟으로 한다.
        if (currentSkill.type == EffectType.Attack)
        {
            target = Player.Instance.gameObject;
        }
        else
        {
            target = gameObject;
        }

        // 준비한 스킬을 사용한다.
        CardInfo.Instance.effects[(int)currentSkill.type](currentSkill.amount, target);
    }

    // 죽는다.
    public override void Die()
    {
        base.Die();

        // 오브젝트 비활성화
        gameObject.SetActive(false);
        // Battle Info에 남은 적 -1
    }
}
