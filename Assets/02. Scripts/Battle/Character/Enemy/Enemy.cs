// 김민철
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [Header("정보")]
    // 적이 사용할 스킬 리스트
    public EnemySkill skillData;

    [Header("컴포넌트")]
    // 행동 정보 아이콘
    protected Image behaviorIcon;
    protected TMP_Text behaviorAmount;

    // 상제정보창
    protected TMP_Text behaviorName;
    protected TMP_Text behaviorDescription;

    public override void Awake()
    {
        base.Awake();

        // 행동 정보 아이콘
        behaviorIcon = transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>();
        behaviorAmount = behaviorIcon.transform.GetChild(0).GetComponent<TMP_Text>();

        // 상제정보창
        behaviorName = statusPanel.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        behaviorDescription = statusPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    protected override void Start()
    {
        base.Start();

        // 테스트는 Start가 제맛
        ReadySkill();

        // BattleInfo에 자신 추가
        BattleInfo.Instance.EnrollEnemy(this);

        // BattleManager에 이벤트 등록
        TurnManager.Instance.onEndEnemyTurn.AddListener(EndEnemyTurn);
        TurnManager.Instance.onStartPlayerTurn.AddListener(ReadySkill);
    }

    public void StartEnemyTurn()
    {
        ReadySkill();
    }

    public void EndEnemyTurn()
    {
        // 플레이어에게 스킬을 사용한다. 이때, 애니메이션이 모두 끝나야 이후 명령들을 시작한다.
        CastSkill();

        // 디버프(출혈 등)가 전부 적용된다.
        GetBleedAll();
    }

    [Header("런타임 변수")]
    // 현재 준비 중인 스킬
    public Skill currentSkill;

    // 스킬 사용을 준비한다.
    public void ReadySkill()
    {
        int i = Random.Range(0, skillData.skills.Length);

        // 1. 랜덤한 스킬들 중 하나를 선택한다.
        currentSkill = skillData.skills[i];

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

    // 죽는다.
    public override void Die()
    {
        // 오브젝트 비활성화
        gameObject.SetActive(false);
        // TurnManager에서 자기 자신의 이벤트를 제거
        TurnManager.Instance.onEndEnemyTurn.RemoveListener(EndEnemyTurn);
        // BattleInfo에서 자기 자신을 제거한다.
        BattleInfo.Instance.DisenrollEnemy(this);
    }
}
