using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : Poolable
{
    private void Start()
    {
        image = transform.GetComponent<Image>();
        tmp_Text = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetContent(Sprite image, string tmp_Text)
    {
        this.image.sprite = image;
        this.tmp_Text.text = tmp_Text;
    }

    public Image image;
    public TMP_Text tmp_Text;
}