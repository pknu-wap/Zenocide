using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : Poolable
{
    public BuffIcon(Image image, TMP_Text tmp_Text)
    {
        this.image = image;
        this.tmp_Text = tmp_Text;
    }

    public Image image;
    public TMP_Text tmp_Text;
}