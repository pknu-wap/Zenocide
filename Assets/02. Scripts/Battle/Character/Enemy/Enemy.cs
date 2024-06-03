// 김민철
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [Header("정보")]
    // 적의 정보
    public EnemyData enemyData;

    [Header("컴포넌트")]
    // 행동 정보 아이콘
    protected Image behaviorIcon;
    protected TMP_Text behaviorAmount;

    // 상제정보창
    protected TMP_Text behaviorName;
    protected TMP_Text behaviorDescription;

    // 스킬 모션, 이펙트
    Sequence skillSequence;
    ParticleSystem healEffect;
    Image shieldMask;

    // 상수
    float fadeDelay = 2f;
    float skillDelay = 0.5f;

    public override void Awake()
    {
        EnrollComponents();
    }

    public void Start()
    {
        GameManager.Instance.onStartBattle.AddListener(StartBattle);
    }

    // 자신의 컴포넌트들을 할당한다.
    protected override void EnrollComponents()
    {
        base.EnrollComponents();

        // 행동 정보 아이콘
        behaviorIcon = transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>();
        behaviorAmount = behaviorIcon.transform.GetChild(0).GetComponent<TMP_Text>();

        // 상제정보창
        behaviorName = statusPanel.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        behaviorDescription = statusPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>();

        // 스킬 이펙트, 모션
        shieldMask = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // 전투를 시작할 때 호출한다.
    protected override void StartBattle()
    {
        // 비활성화된 적은 이벤트를 등록하지 않는다.
        if(gameObject.activeSelf == false)
        {
            return;
        }

        base.StartBattle();

        // BattleInfo에 자신 추가
        BattleInfo.Instance.EnrollEnemy(this);

        // BattleManager에 이벤트 등록
        TurnManager.Instance.onStartPlayerTurn.AddListener(ReadySkill);
    }

    public void EndEnemyTurn()
    {
        // 디버프(출혈 등)가 전부 적용된다.
        GetBleedAll();

        // 죽고 나서 스킬 사용하는 걸 방지
        if(currentHp <= 0)
        {
            return;
        }

        // 스킬을 사용한다. 이때, 애니메이션이 모두 끝나야 이후 명령들을 시작한다.
        CastSkill();
    }

    // 적 정보를 갱신한다.
    public void UpdateEnemyData(EnemyData data)
    {
        enemyData = data;

        // 이미지 변경
        imageComponent.sprite = enemyData.illust;

        // 최대 HP 변경 및 현재 체력을 maxHp와 같게 변경
        maxHp = enemyData.maxHp;
        currentHp = maxHp;
        UpdateCurrentHP();
    }

    [Header("런타임 변수")]
    // 현재 준비 중인 스킬
    public Skill currentSkill;

    // 스킬 사용을 준비한다.
    public void ReadySkill()
    {
        int i = Random.Range(0, enemyData.skills.Length);

        // 1. 랜덤한 스킬들 중 하나를 선택한다.
        currentSkill = enemyData.skills[i];

        // 2.UI를 갱신한다.
        // 2-1. 자신이 고른 스킬을 체력바 위에 표시한다.
        behaviorIcon.sprite = CardInfo.Instance.skillIcons[(int)currentSkill.type];
        behaviorAmount.text = currentSkill.amount.ToString();

        // 2-2. 상세정보창을 스킬의 설명으로 갱신한다.
        SkillText skillText = CardInfo.Instance.GetSkillText(currentSkill);
        behaviorName.text = skillText.name;
        behaviorDescription.text = skillText.description;
    }

    // 스킬을 사용한다.
    public void CastSkill()
    {
        // 타겟을 받아온다.
        Character[] target = CardInfo.Instance.GetTarget(currentSkill.target, this);

        // 준비한 스킬을 사용한다.
        CardInfo.Instance.ActivateSkill(currentSkill, target);
    }

    // SkillType 별 모션 출력
    public IEnumerator SkillMotion()
    {
        // 죽고 나서 스킬 사용 방지
        if (currentHp <= 0)
        {
            yield break;
        }

        switch (currentSkill.type)
        {
            case (SkillType.Attack):
                skillSequence = DOTween.Sequence()
                    .Append(transform.DOScale(1.5f, skillDelay * 2 / 5))
                    .Append(imageComponent.transform.DOShakePosition(skillDelay * 1 / 5, 100f))
                    .Append(transform.DOScale(0.9f, skillDelay * 2 / 5));    // enemy 원래 스케일이 0.9로 돼있다.
                break;
            case (SkillType.Shield):
                skillSequence = DOTween.Sequence()
                    .Append(shieldMask.DOFade(0.7f, skillDelay / 2))
                    .Append(shieldMask.DOFade(0f, skillDelay / 2));
                break;
        }

        yield return new WaitForSeconds(skillDelay);
    }

    // 죽는다.
    public override void Die()
    {
        // TurnManager에서 자기 자신의 이벤트를 제거
        TurnManager.Instance.onStartPlayerTurn.RemoveListener(ReadySkill);

        // BattleInfo에서 자기 자신을 제거한다.
        BattleInfo.Instance.DisenrollEnemy(this);

        StartCoroutine(DieMotionCo());
    }

    IEnumerator DieMotionCo()
    {
        imageComponent.DOFade(0f, fadeDelay);

        yield return new WaitForSeconds(fadeDelay);

        // 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}
