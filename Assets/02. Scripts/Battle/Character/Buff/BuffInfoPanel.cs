using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffInfoPanel : Poolable
{
    private void Start()
    {
        name = transform.GetChild(0).GetComponent<TMP_Text>();
        description = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetContent(string name, string description)
    {
        this.name.text = name;
        this.description.text = description;
    }

    public new TMP_Text name;
    public TMP_Text description;
}