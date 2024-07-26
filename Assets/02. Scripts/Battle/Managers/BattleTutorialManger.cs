using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTutorialManger : MonoBehaviour
{
    // 싱글톤
    public static BattleTutorialManger Instance;
    void Awake() => Instance = this;
}
