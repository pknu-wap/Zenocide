using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("데이터")]
    // HP(체력)
    protected int currentHp = 100;
    protected int maxHp = 100;

    [Header("컴포넌트")]
    [SerializeField]
    protected Image hpBar;
    [SerializeField]
    protected TMP_Text hpText;

    public void Awake()
    {
        hpBar = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        hpText = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();

        UpdateCurrentHP();
    }

    // 이 오브젝트의 hp를 반환한다.
    public int GetHP()
    {
        return currentHp;
    }

    public void UpdateCurrentHP()
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
        hpText.text = currentHp + "/" + maxHp;
    }

    // 이 오브젝트의 hp를 감소시킨다.
    public void DecreaseHP(int damage)
    {
        // hp를 damage만큼 감소시킨다.
        currentHp -= damage;

        UpdateCurrentHP();

        // hp가 0 이하가 될 경우
        if (currentHp <= 0)
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
