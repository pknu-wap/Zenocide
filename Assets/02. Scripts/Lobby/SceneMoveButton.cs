using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneMoveButton : MonoBehaviour
{
    string dataFileName = "Data.json";
    // 경고창 (진짜로 할 것임?)
    [SerializeField] GameObject alertPanel;
    [SerializeField] bool isLoadButton = false;

    private bool isMoved = false;

    private void Start()
    {
        if(alertPanel != null)
        {
            alertPanel.SetActive(false);
        }

        // 세이브 파일이 없으면 로드 버튼 비활성화
        if (isLoadButton && !DataManager.Instance.IsFileExist())
        {
            OffButton();
        }
    }

    public void MoveScene(string sceneName)
    {
        // 연속으로 눌러지는 걸 막는다.
        if (isMoved)
        {
            return;
        }

        // 씬 전환 후 데이터를 삽입 여부를 알기 위한 불리언
        DataManager.Instance.isLoaded = isLoadButton;

        SceneLoader.Instance.LoadScene(sceneName);
        isMoved = true;
    }

    public void OffButton()
    {
        // 버튼 투명도
        Color color = GetComponent<Image>().color;
        color.a = 0.5f;
        GetComponent<Image>().color = color;

        // 버튼 텍스트 투명도
        GetComponentInChildren<TMP_Text>().alpha = 0.5f;
    }

    public void ToggleAlertPanel()
    {
        if (isLoadButton && !DataManager.Instance.IsFileExist())
        {
            return;
        }

        alertPanel.SetActive(!alertPanel.activeSelf);
    }
}
