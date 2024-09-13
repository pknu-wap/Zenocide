using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffInfoPanel : Poolable
{
    public BuffInfoPanel(TMP_Text name, TMP_Text description)
    {
        this.name = name;
        this.description = description;
    }
    public new TMP_Text name;
    public TMP_Text description;
}