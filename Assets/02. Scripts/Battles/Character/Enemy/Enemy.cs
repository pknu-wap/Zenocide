// 김민철
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [Header("정보")]
    // 적이 사용할 스킬 리스트
    public SkillData skillData;

    [Header("컴포넌트")]
    // 행동 정보 아이콘
    [SerializeField] protected Image behaviorIcon;
    [SerializeField] protected TMP_Text behaviorAmount;

    // 상제정보창
    [SerializeField] protected TMP_Text behaviorName;
    [SerializeField] protected TMP_Text behaviorDescription;

    public override void Awake()
    {
        base.Awake();

        // 행동 정보 아이콘
        behaviorIcon = transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Image>();
        behaviorAmount = transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>();

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
        BattleInfo.Inst.IncreaseEnemyCount();

        // BattleManager에 이벤트 등록
        TurnManager.Inst.onEndEnemyTurn.AddListener(EndEnemyTurn);
        TurnManager.Inst.onStartPlayerTurn.AddListener(ReadySkill);
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
        behaviorName.text = currentSkill.skillName;
        behaviorDescription.text = currentSkill.description;
    }

    // 스킬을 사용한다.
    public void CastSkill()
    {
        Character target = EnemySkillInfo.Instance.ReturnTarget(currentSkill.type, this);

        // 준비한 스킬을 사용한다.
        EnemySkillInfo.Instance.effects[(int)currentSkill.type](currentSkill.amount, currentSkill.turnCount, target);
    }

    // 죽는다.
    public override void Die()
    {
        // 오브젝트 비활성화
        gameObject.SetActive(false);
        // BattleManager에서 자기 자신 제거
        TurnManager.Inst.onEndEnemyTurn.RemoveListener(EndEnemyTurn);
        // Battle Info에 남은 적 -1
        BattleInfo.Inst.DecreaseEnemyCount();
    }
}
