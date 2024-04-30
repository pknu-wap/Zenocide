// ���ö
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [Header("����")]
    // ���� ����� ��ų ����Ʈ
    public SkillData skillData;

    [Header("������Ʈ")]
    // �ൿ ���� ������
    [SerializeField] protected Image behaviorIcon;
    [SerializeField] protected TMP_Text behaviorAmount;

    // ��������â
    [SerializeField] protected TMP_Text behaviorName;
    [SerializeField] protected TMP_Text behaviorDescription;

    public override void Awake()
    {
        base.Awake();

        // �ൿ ���� ������
        behaviorIcon = transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Image>();
        behaviorAmount = transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>();

        // ��������â
        behaviorName = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        behaviorDescription = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    private void Start()
    {
        // �׽�Ʈ�� Start�� ����
        ReadySkill();

        // BattleManager�� �̺�Ʈ ���
        BattleManager.Instance.onEndEnemyTurn.AddListener(CastSkill);
        BattleManager.Instance.onStartPlayerTurn.AddListener(ReadySkill);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DecreaseHP(6);
        }
    }

    public void StartEnemyTurn()
    {
        ReadySkill();
    }

    public void EndEnemyTurn()
    {
        // �÷��̾�� ��ų�� ����Ѵ�.
        CastSkill();

        // �����(���� ��)�� ���� ����ȴ�.
        GetBleedAll();
    }

    [Header("��Ÿ�� ����")]
    // ���� �غ� ���� ��ų
    public Skill currentSkill;

    // ��ų ����� �غ��Ѵ�.
    public void ReadySkill()
    {
        int i = Random.Range(0, skillData.skills.Length);

        // 1. ������ ��ų�� �� �ϳ��� �����Ѵ�.
        currentSkill = skillData.skills[i];

        // 2.UI�� �����Ѵ�.
        // 2-1. �ڽ��� ���� ��ų�� ü�¹� ���� ǥ���Ѵ�.
        behaviorIcon.sprite = CardInfo.Instance.skillIcons[(int)currentSkill.type];
        behaviorAmount.text = currentSkill.amount.ToString();

        // 2-2. ������â�� ��ų�� �������� �����Ѵ�.
        behaviorName.text = currentSkill.skillName;
        behaviorDescription.text = currentSkill.description;
    }

    // ��ų�� ����Ѵ�.
    public void CastSkill()
    {
        Character target = EnemySkillInfo.Instance.ReturnTarget(currentSkill.type, this);

        // �غ��� ��ų�� ����Ѵ�.
        EnemySkillInfo.Instance.effects[(int)currentSkill.type](currentSkill.amount, currentSkill.turnCount, target);
    }

    // �״´�.
    public override void Die()
    {
        base.Die();

        // ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        // BattleManager���� �ڱ� �ڽ� ����
        BattleManager.Instance.onEndEnemyTurn.RemoveListener(CastSkill);
        // Battle Info�� ���� �� -1
    }
}