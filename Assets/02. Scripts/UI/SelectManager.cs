using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChoiceType
{
    Up,
    Down
}

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    public GameObject UpSignLeft;            //?úÑ ?Ñ†?Éù ?ëú?ãú(?ôºÏ™?)
    public GameObject UpSignRight;           //?úÑ ?Ñ†?Éù ?ëú?ãú(?ò§Î•∏Ï™Ω)
    public GameObject DownSignLeft;          //?ïÑ?ûò ?Ñ†?Éù ?ëú?ãú(?ôºÏ™?)
    public GameObject DownSignRight;         //?ïÑ?ûò ?Ñ†?Éù ?ëú?ãú(?ò§Î•∏Ï™Ω)

    public GameObject ChoiceBoxUp;
    public GameObject ChoiceBoxDown;

    public GameObject ChoiceUp;
    public GameObject ChoiceDown;

    public GameObject Dialogue;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if (instance != null) return;
        DontDestroyOnLoad(gameObject);
    }

    public Choice currentChoice;

    void Start(){
        UpSignLeft.SetActive(false);
        UpSignRight.SetActive(false);
        DownSignLeft.SetActive(false);
        DownSignRight.SetActive(false);
    }

    void Update(){
        if(currentChoice != null){

            if(Choice.Selected != 0){
                ChoiceUp.SetActive(false);
                ChoiceDown.SetActive(false);
                Dialogue.SetActive(true);
            }
            
            if (currentChoice.name == "ChoiceBoxUp")
            {
                UpSignLeft.SetActive(true);
                UpSignRight.SetActive(true);
                DownSignLeft.SetActive(false);
                DownSignRight.SetActive(false);
            }
            else
            {
                UpSignLeft.SetActive(false);
                UpSignRight.SetActive(false);
                DownSignLeft.SetActive(true);
                DownSignRight.SetActive(true);
            }
        }
        
    }    
}
