using UnityEngine;
using UnityEngine.UI;

public class StatusHP : MonoBehaviour
{

    public Slider slider;

    void Update()
    {
        slider.value = Player.Instance.GetCurrentHP() / Player.Instance.GetMaxHP();
    }

}