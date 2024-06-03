using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance { get; set; }

    [Header("스크립트 에셋")]
    // CSV 내용이 담겨 있는 딕셔너리 리스트
    private List<Dictionary<string, object>> diaryDialog;
    [SerializeField] private TextAsset diaryCSV;

    [Header("컴포넌트")]
    [SerializeField] private PageCurl book;
    [SerializeField] private Transform bookObject;
    [SerializeField] private PageScripter[] scripter;
    [SerializeField] private Transform choiceParent;
    [SerializeField] private TMP_Text[] choiceButtons;

    [Header("현재 다이얼로그 정보")]
    // 현재 출력 중인 텍스트 번호
    public int currentDialogIndex = 0;
    // 마지막으로 출력한 (혹은 중인) 텍스트 번호, 중복 출력 방지에 쓰인다.
    public int lastDialogIndex = -1;
    // 현재 출력 중인 페이지 번호
    public int currentPageIndex = 0;
    // 선택된 단어
    [SerializeField] private string selectedWord;

    [Header("보급병")]
    public Supplier supplier;

    private void Awake()
    {
        // 싱글톤
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Scripter 들을 받아온다.
        scripter = new PageScripter[bookObject.childCount];
        for (int i = 0; i < bookObject.childCount; ++i)
        {
            scripter[i] = bookObject.GetChild(bookObject.childCount - 1 - i).GetComponent<PageScripter>();
        }
        // 스크립트도 받아온다.
        book = bookObject.GetComponent<PageCurl>();

        // 선택지 오브젝트들을 캐싱해둔다.
        choiceButtons = new TMP_Text[choiceParent.childCount];
        for(int i = 0; i < choiceParent.childCount; ++i)
        {
            choiceButtons[i] = choiceParent.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
        }
        choiceParent.gameObject.SetActive(false);

        // CSV를 읽어온다.
        diaryDialog = CSVReader.Read(diaryCSV);
    }

    private void Start()
    {
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        yield return StartCoroutine(book.FlipPage());
        ++currentPageIndex;
    }

    // 문장을 넘겨준다.
    public void AddDialog()
    {
        // 페이지가 넘어가고 있다면 무조건 취소한다.
        if (book.isCurling == true)
        {
            return;
        }

        // 이미 타이핑 중일 때
        if (scripter[currentPageIndex].isTyping == true)
        {
            // 캔슬은 하지 않았다면
            if(scripter[currentPageIndex].cancelTyping == false)
            {
                // 캔슬한다.
                scripter[currentPageIndex].CancelTyping();
            }

            // 그 외의 경우는 함수를 취소한다.
            return;
        }

        // 해당 번호가 출력 중이라면 무조건 취소한다.
        if (lastDialogIndex == currentDialogIndex)
        {
            return;
        }

        // 마지막 출력 번호를 갱신하고
        lastDialogIndex = currentDialogIndex;
        // 출력한다.
        StartCoroutine(scripter[currentPageIndex].ShowDialog(diaryDialog[currentDialogIndex]));
    }

    public bool isSelected = false;

    // 선택지 버튼을 띄운다.
    public IEnumerator ShowChoiceButton()
    {
        // 선택지를 띄운다.
        for(int i = 0; i < choiceButtons.Length; ++i)
        {
            choiceButtons[i].text = diaryDialog[currentDialogIndex][i.ToString()].ToString();
        }
        choiceParent.gameObject.SetActive(true);

        // 선택할 때까지 기다린다.
        while (isSelected == false)
        {
            yield return null;
        }

        // scripter에 전달해준다.
        scripter[currentPageIndex].Select(selectedWord);

        // 다시 false로 돌리고 종료
        isSelected = false;
    }

    // 선택한다.
    public void Select(int i)
    {
        selectedWord = diaryDialog[currentDialogIndex][i.ToString()].ToString();

        // 선택한 직업의 카드를 추가한다.
        string[] classCards = supplier.classCardDeck[selectedWord];
        for(int j = 0; j < classCards.Length; ++j)
        {
            CardManager.Instance.AddCardToDeck(classCards[j]);
        }

        // 선택한 직업의 아이템을 추가한다.
        string[] classItems = supplier.classCardDeck[selectedWord];
        for (int j = 0; j < classItems.Length; ++j)
        {
            Items.Instance.AddItem(classItems[j]);
        }

        choiceParent.gameObject.SetActive(false);
        isSelected = true;
    }
}
