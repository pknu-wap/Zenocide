using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : Poolable
{
    public Image image;
    public TMP_Text turnCount;

    /*void Start()
    {
        sprite = transform.GetChild(0).GetComponent<Sprite>();
        turnCount = transform.GetChild(1).GetComponent<TMP_Text>();
    }*/

    public void SetContent(Sprite image, string tmp_Text)
    {
        this.image.sprite = image;
        this.turnCount.text = tmp_Text;
    }

}