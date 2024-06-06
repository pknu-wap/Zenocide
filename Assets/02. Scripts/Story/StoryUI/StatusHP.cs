using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusHP : MonoBehaviour
{
    public Slider slider;

    [Header("현재 HP 텍스트 ")]
    public TMP_Text currentHPText;

    public void UpdateHPUI()
    {
        int currentHP = Player.Instance.GetCurrentHP();
        int maxHP = Player.Instance.GetMaxHP();
        currentHPText.text = (currentHP + "/" + maxHP).ToString();
        slider.value = (float)currentHP / maxHP;
    }
}