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
    [SerializeField] protected Transform panelContainer;
    protected TMP_Text behaviorName;
    protected TMP_Text behaviorDescription;

    // 스킬 모션, 이펙트
    Sequence skillSequence;
    Image effectMask;

    // 콜라이더
    Collider2D enemyCollider;

    [Header("상수")]
    float fadeDelay = 2f;
    float skillDelay = 0.5f;   // 0.5의 배수로 해줘야 함

    public void Awake()
    {
        EnrollComponents();
    }

    // 자신의 컴포넌트들을 할당한다.
    protected override void EnrollComponents()
    {
        base.EnrollComponents();

        // 행동 정보 아이콘
        behaviorIcon = transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>();
        behaviorAmount = behaviorIcon.transform.GetChild(0).GetComponent<TMP_Text>();

        // 상제정보창
        behaviorName = panelContainer.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        behaviorDescription = panelContainer.GetChild(0).GetChild(1).GetComponent<TMP_Text>();

        // 스킬 이펙트, 모션
        effectMask = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();

        // 콜라이더
        enemyCollider = GetComponent<Collider2D>();
    }

    // 적을 등록한다.
    public void EnrollEnemy(EnemyData data)
    {
        // null이 들어오면 비활성화하고 종료
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // BattleInfo에 자신 추가
        BattleInfo.Instance.EnrollEnemy(this);

        // 이벤트 등록
        TurnManager.Instance.onEndEnemyTurn.AddListener(EndEnemyTurn);

        TurnManager.Instance.onStartPlayerTurn.AddListener(ReadySkill);

        // 데이터를 넣고
        enemyData = data;

        // 이미지 변경
        imageComponent.sprite = enemyData.illust;

        // 최대 HP 변경
        maxHp = enemyData.maxHp;

        // 상태 초기화
        ResetState();
        
        // 콜라이더를 켜 상호작용을 시작하고
        enemyCollider.enabled = true;

        // 활성화한다.
        gameObject.SetActive(true);
    }

    // 적 상태를 초기화한다.
    public override void ResetState()
    {
        base.ResetState();

        currentHp = maxHp;
        UpdateHPUI();
        imageComponent.color = Color.white;
    }

    public void EndEnemyTurn()
    {
        // 디버프(출혈 등)가 전부 적용된다.
        GetBuffAll();

        // 죽고 나서 스킬 사용하는 걸 방지
        if(currentHp <= 0)
        {
            return;
        }

        // 스킬을 사용한다. 이때, 애니메이션이 모두 끝나야 이후 명령들을 시작한다.
        CastSkill();
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
        CardInfo.Instance.ActivateSkill(currentSkill, target, this);
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
                yield return skillSequence.WaitForCompletion();
                break;

            case (SkillType.Shield):
                effectMask.color = new Color(92f / 255, 206f / 255, 299f / 255);
                skillSequence = DOTween.Sequence()
                    .Append(effectMask.DOFade(0.7f, skillDelay / 2))
                    .Append(effectMask.DOFade(0f, skillDelay / 2));
                yield return skillSequence.WaitForCompletion();
                break;

            case (SkillType.Heal):
                effectMask.color = new Color(47f / 255, 144f / 255, 51f / 255);
                skillSequence = DOTween.Sequence()
                    .Append(effectMask.DOFade(0.7f, skillDelay / 2))
                    .Append(effectMask.DOFade(0f, skillDelay / 2));
                yield return skillSequence.WaitForCompletion();
                break;

            case (SkillType.ExtraDamage):
            case (SkillType.LingeringExtraDamage):
            case (SkillType.ExtraBleedDamage):
            case (SkillType.LingeringExtraBleedDamage):
                effectMask.color = new Color(207f/255, 54f / 255, 36f / 255 );
                skillSequence = DOTween.Sequence()
                    .Append(effectMask.DOFade(0.7f, skillDelay / 2))
                    .Append(effectMask.DOFade(0f, skillDelay / 2));
                yield return skillSequence.WaitForCompletion();
                break;

            default:
                break;
        }

        // yield return new WaitForSeconds(skillDelay);
    }

    public override void DecreaseHP(int damage)
    {
        // 현재 데미지
        int currentDamage = damage;

        // 실드가 있다면 데미지 재계산
        if (shield > 0)
        {
            // currentDamage를 감소시키고
            currentDamage -= shield;
            if (currentDamage < 0)
            {
                // 잔여 데미지가 음수면 0으로 적용한다.
                currentDamage = 0;
            }

            // 실드에선 기존 데미지를 뺀다.
            shield -= damage;
            if (shield < 0)
            {
                // 잔여 방어막이 음수면 0으로 적용한다.
                shield = 0;
            }
        }

        // hp를 잔여 데미지 만큼 감소시킨다.
        currentHp -= currentDamage;

        // UI를 갱신한다.
        UpdateShieldUI();
        UpdateHPUI();

        // 적이 피격될 때 모션, 데미지 텍스트 출력
        if (currentDamage > 0)
        {
            DamageText damageText = Instantiate(damageTextPrefab, transform.GetChild(0)).GetComponent<DamageText>();
            StartCoroutine(damageText.PrintDamageText(currentDamage));
            imageComponent.transform.DOShakePosition(0.3f, 4f * currentDamage);
            effectMask.color = Color.red;
            skillSequence = DOTween.Sequence()
                    .Append(effectMask.DOFade(0.7f, 0.3f))
                    .Append(effectMask.DOFade(0f, 0.3f));
        }

        // hp가 0 이하가 될 경우
        if (currentHp <= 0)
        {
            currentHp = 0;
            UpdateHPUI();

            // 죽음 이벤트 실행
            Die();
        }
    }

    // 죽는다.
    public override void Die()
    {
        // 카드 대상이 되지 않게 콜라이더를 비활성화한다.
        enemyCollider.enabled = false;

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
