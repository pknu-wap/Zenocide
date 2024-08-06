using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLog : MonoBehaviour
{
    [Header("TMP Text")]
    public TMP_Text logName;
    public TMP_Text logText;

    [Header("로그 박스 오브젝트")]
    public Image logBox;

    [Header("BG 오브젝트")]
    public GameObject nameBG;

    [Header("획득, 소비 아이템 오브젝트")]
    public GameObject equipItem;
    public GameObject usedItem;

    [Header("선택지 확인 변수")]
    private const int notSelected = -1;

    [Header("빈 문자 변수")]
    private const string empty = "";

    public void UpdateTextLog(Dictionary<string, object> log,int choiceResult)
    {
        string name = choiceResult is notSelected ? log["Name"].ToString() : "선택지";
        string text = choiceResult is notSelected ? log["Text"].ToString() : log["Choice" + choiceResult].ToString();
        string eCard = log["Equip Card"].ToString();
        string eItem = log["Equip Item"].ToString();
        string uItem = choiceResult is notSelected ? empty : log["Remove Item" + choiceResult].ToString();

        //로그의 이름과 대사 오브젝트에 할당
        logName.text = name;
        logText.text = text;
        //선택지일 경우 로그의 색 변경
        logName.color = choiceResult is notSelected ? Color.white : new Color(1,0.388f,0.278f);
        logText.color = choiceResult is notSelected ? Color.white : new Color(1,0.388f,0.278f);

        if(name is empty)
        {
            //나레이션이므로 이름 뒤에 표시되던 BG 비활성화
            nameBG.SetActive(false);
            logText.alignment = TextAlignmentOptions.TopLeft;
        }
        
        equipItem.SetActive(eCard is not empty || eItem is not empty ? true : false);
        usedItem.SetActive(uItem is not empty ? true : false);

        //획득한 아이템이나 카드가 있는 경우 Text 오브젝트 활성화
        if(eCard is not empty || eItem is not empty)
        {
            //획득 아이템과 카드를 string 변수에 저장
            string equip = log["Equip Item"].ToString() + " " + log["Equip Card"].ToString();
            TMP_Text EquipTMP = equipItem.transform.GetChild(0).GetComponent<TMP_Text>();
            EquipTMP.text = equip;
            EquipTMP.color = Color.green;
        }
        
        //사용한 아이템이 있는 경우 Text 오브젝트 활성화
        if(uItem is not empty)
        {
            TMP_Text UsedTMP = usedItem.transform.GetChild(0).GetComponent<TMP_Text>();
            UsedTMP.text = uItem;
            UsedTMP.color = Color.red;
        } 
        
    }
}
