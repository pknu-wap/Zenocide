using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : Poolable
{
    public Image image;
    public TMP_Text turnCount;

    public void SetContent(Sprite image, string tmp_Text)
    {
        this.image.sprite = image;
        this.turnCount.text = tmp_Text;
    }

}