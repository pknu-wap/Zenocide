using TMPro;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject[] Tabs;
    public TMP_Text[] TabButtonTexts;

    public Color deselectedColor = new Color(1f, 1f, 1f);
    public Color selectedColor = new Color(0f, 0f, 0f);

    public Transform floatingCanvas;
    public Transform uiCanvas;
    public GameObject optionPanel;

    private void Start()
    {
        // 시작 시 첫 번째 탭으로 전환
        SwitchToTab(0);
    }

    public void SwitchToTab(int TabID)
    {
        // 모든 탭을 비활성화
        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].SetActive(false);
            // 비선택된 버튼 색상으로 변경
            TabButtonTexts[i].color = deselectedColor;
        }
        // 선택된 탭을 활성화
        Tabs[TabID].SetActive(true);
        // 선택된 버튼 색상으로 변경
        TabButtonTexts[TabID].color = selectedColor;
    }

    public void MoveOptionPanelToStoryFloatingCanvas()
    {
        // 옵션 패널의 부모를 Floating Canvas로 설정
        optionPanel.transform.SetParent(floatingCanvas, false);
    }

    public void MoveOptionPanelToBattleUICanvas()
    {
        // 옵션 패널의 부모를 UI Canvas로 설정
        optionPanel.transform.SetParent(uiCanvas, false);
    }
}

