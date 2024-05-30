// ���ö
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class BleedEffect
{
    public BleedEffect(SkillType type, int damagePerTurn, int remainingTurns)
    {
        this.type = type;
        this.damagePerTurn = damagePerTurn;
        this.remainingTurns = remainingTurns;
    }

    public SkillType type;
    public int damagePerTurn;
    public int remainingTurns;
}

public class DebuffIconComponent
{
    public DebuffIconComponent(Image image, TMP_Text tmp_Text)
    {
        this.image = image;
        this.tmp_Text = tmp_Text;
    }

    public Image image;
    public TMP_Text tmp_Text;
}

public class Character : MonoBehaviour
{
    [Header("������")]
    // HP(ü��)
    [SerializeField] protected int currentHp = 100;
    [SerializeField] public int maxHp = 100;

    // ����׿�, ���� ����
    [Header("������Ʈ")]
    // HP ��
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TMP_Text hpText;
    // ���� ������ ������ ���� ���� -> ������Ʈ Ǯ������ ��ü
    [SerializeField] protected Transform statusPanel;
    // �����â
    [SerializeField] public List<DebuffIconComponent> debuffIcons;
    [SerializeField] protected TMP_Text[] debuffName;
    [SerializeField] protected TMP_Text[] debuffDescription;

    [Header("�����̻�")]
    public Transform debuffIconContainer;
    [SerializeField] public List<BleedEffect> debuffs;

    [Header("�̺�Ʈ")]
    [SerializeField] public UnityEvent onTurnStarted;

    public virtual void Awake()
    {
        // HP ��
        hpBar = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        hpText = hpBar.transform.GetChild(0).GetComponent<TMP_Text>();

        // ����� ȿ����(���� ������)�� ��Ƶ� ����Ʈ
        debuffs = new List<BleedEffect>();
        debuffIcons = new List<DebuffIconComponent>();

        // ����� �����ܵ��� �θ� �����̳�
        debuffIconContainer = transform.GetChild(0).GetChild(1).GetChild(1);
        // debuffIconContainer�� ��� �ڽ� ������Ʈ�� ��Ȱ��ȭ. (�ڽ��� �ڽ��� X)
        foreach (Transform icon in debuffIconContainer)
        {
            // debuffIcons�� icon�� �̹���, �ؽ�Ʈ ������Ʈ�� �Ҵ�
            debuffIcons.Add(new DebuffIconComponent(icon.GetComponent<Image>(), icon.GetChild(0).GetComponent<TMP_Text>()));
            // �׸��� ��Ȱ��ȭ.
            icon.gameObject.SetActive(false);
        }

        // ����� ������â�� �ҷ��´�. (����, �ൿ����â ����)
        statusPanel = transform.GetChild(0).GetChild(2);
        debuffName = new TMP_Text[statusPanel.childCount - 1];
        debuffDescription = new TMP_Text[statusPanel.childCount - 1];

        for (int i = 1; i < statusPanel.childCount; ++i)
        {
            debuffName[i - 1] = statusPanel.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            debuffDescription[i - 1] = statusPanel.GetChild(i).GetChild(1).GetComponent<TMP_Text>();
        }
    }

    protected virtual void Start()
    {
        UpdateCurrentHP();

        UpdateAllDebuffIcon();
    }

    // �� ������Ʈ�� hp�� ��ȯ�Ѵ�.
    public int GetHP()
    {
        return currentHp;
    }

    public void UpdateCurrentHP()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
        hpText.text = currentHp + "/" + maxHp;
    }

    // �� ������Ʈ�� hp�� ���ҽ�Ų��.
    public void DecreaseHP(int damage)
    {
        // hp�� damage��ŭ ���ҽ�Ų��.
        currentHp -= damage;

        UpdateCurrentHP();

        // hp�� 0 ���ϰ� �� ���
        if (currentHp <= 0)
        {
            // ���� �̺�Ʈ ����
            Die();
        }
    }

    // �� ������Ʈ�� hp�� ���ҽ�Ų��.
    public void IncreaseHP(int heal)
    {
        // hp�� damage��ŭ ���ҽ�Ų��.
        currentHp += heal;

        // hp�� �ִ�ġ �̻��� �� ���
        if (currentHp > maxHp)
        {
            // �ִ�ġ�� ���߱�
            currentHp = maxHp;
        }

        // UI ����
        UpdateCurrentHP();
    }

    public virtual void Die()
    {
        // ������ ���õ� ȿ�� ó��
        // ���� �ִϸ��̼�
    }

    // ���� ȿ���� �� ���� �̺�Ʈ�� ����Ѵ�.
    public void EnrollBleed(BleedEffect bleedEffect)
    {
        // ���� ����Ʈ�� �߰�
        debuffs.Add(bleedEffect);

        // ���� ����� UI �߰�
        int i = debuffs.Count - 1;

        UpdateDebuffIcon(i);
    }

    // ������� ���� �����Ѵ�.
    public void CleanseDebuff()
    {
        debuffs.Clear();

        UpdateAllDebuffIcon();
    }

    public void UpdateDebuffIcon(int index)
    {
        /*
         * �����ϴ� ���� ����� ������ �����Ǿ� �ִ�. (�װ� �ۿ� �� �׷���...)
         */
        // i��° �����ܿ� ���ڸ� �����ϰ�
        debuffIcons[index].image.sprite = CardInfo.Instance.debuffIcons[0];
        debuffIcons[index].tmp_Text.text = debuffs[index].remainingTurns.ToString();

        // i��° �����â�� ������ �����Ѵ�
        debuffName[index].text = DebuffInfo.debuffNameDict[debuffs[index].type];
        // �̷��� $�� {}�� ���� �������� ���ڿ��� ��� �� �ִ�.
        debuffDescription[index].text = $"{debuffs[index].remainingTurns}{DebuffInfo.debuffDescriptionDict[debuffs[index].type]}";

        // ������Ʈ Ȱ��ȭ
        // �� �������� �����丵�� �ʿ��غ��δ�.
        debuffIconContainer.GetChild(index).gameObject.SetActive(true);
        debuffName[index].gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateAllDebuffIcon()
    {
        // ��� ���� ������� ����(i��° ������� ����)
        int i = 0;
        for (; i < debuffs.Count; ++i)
        {
            UpdateDebuffIcon(i);
        }

        // ������� ���� �����ܵ���
        for (; i < debuffIconContainer.childCount; ++i)
        {
            // ��Ȱ��ȭ�Ѵ�.
            debuffIconContainer.GetChild(i).gameObject.SetActive(false);
            debuffName[i].gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    // ���� ȿ���� �߻���Ų��.
    public void GetBleedAll()
    {
        // �ް� �� ��ü ������
        int totalDamage = 0;

        // ��� ���� ���� ���� ȿ���� ����
        for(int i = 0; i < debuffs.Count; ++i)
        {
            totalDamage += debuffs[i].damagePerTurn;

            // ���� �� 1 ����
            --debuffs[i].remainingTurns;
            // ���� ���� 0 ���϶��
            if (debuffs[i].remainingTurns <= 0)
            {
                // �ش� ���� ȿ���� �����Ѵ�.
                debuffs.RemoveAt(i);
                // ���� ��������� 1ĭ�� ������ ����������, �ε����� 1 ������ ����
                --i;
            }
        }

        // ���� ����Ʈ ���

        // ü���� ���ҽ�Ų��.
        DecreaseHP(totalDamage);

        // �������� ������Ʈ �Ѵ�.
        UpdateAllDebuffIcon();
    }
}
