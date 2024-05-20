using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusHP : MonoBehaviour
{
    public float maxHP = 100;

    public GameObject Script;
    public Slider slider;

    void Update()
    {
        //slider.value = Script.GetComponent<Character>().GetHP()/maxHP;
        slider.value = 50 / maxHP;
    }

}