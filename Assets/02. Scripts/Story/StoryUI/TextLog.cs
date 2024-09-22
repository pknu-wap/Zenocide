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
        string uItem = log["Remove Item"].ToString();

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
            string equip = log["Equip Item"].ToString() + "#" + log["Equip Card"].ToString();
            TMP_Text EquipTMP = equipItem.transform.GetChild(0).GetComponent<TMP_Text>();
            EquipTMP.text = CompressWithCount(equip);
            EquipTMP.color = Color.green;
        }
        
        //사용한 아이템이 있는 경우 Text 오브젝트 활성화
        if(uItem is not empty)
        {
            TMP_Text UsedTMP = usedItem.transform.GetChild(0).GetComponent<TMP_Text>();
            UsedTMP.text = CompressWithCount(uItem);
            UsedTMP.color = Color.red;
        } 
        
    }

    /// <summary>
    /// #으로 연결된 문자열을 받아 개수와 함께 압축한다.
    /// </summary>
    /// <param name="str">#으로 연결된 문자열</param>
    /// <returns>이름, 개수가 정리된 문자열</returns>
    private string CompressWithCount(string str, bool isItem = true)
    {
        // 먼저 문자열을 분리해 Dictionary로 정리한다.
        Dictionary<string, int> items = Items.Instance.ItemStringToDictionary(str.Split('#'));
        // 결과를 저장할 문자열
        string result_str = "";

        // 첫번째 문자열인지 검사하는 변수
        bool isFirst = true;

        foreach (var item in items)
        {
            // 첫번째만 제외하고
            if (isFirst == false)
            {
                // 콤마를 찍는다.
                result_str += ", ";
            }

            // 한 번 쓴 후 false로
            isFirst = false;

            // 이름을 적어주고
            result_str += item.Key;

            // 아이템이 1개 이하라면
            if (item.Value <= 1)
            {
                // 수량을 적지 않고 끝낸다.
                continue;
            }

            // 아이템이자 능력이라면
            if (isItem == true && ItemInfo.Instance.GetItem(tag).type == ItemType.Status)
            {
                // Lv를 붙인다.
                result_str += " Lv. ";
            }
            // 카드거나 물품이라면
            else
            {
                // x를 붙인다.
                result_str += " x ";
            }

            // 요구하는 개수를 적어준다.
            result_str += item.Value;
        }

        // 만들어진 문자열을 반환한다.
        return result_str;
    }
}
