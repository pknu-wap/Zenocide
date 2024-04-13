// ±ËπŒ√∂
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DecreaseHP(10);
        }
    }
    public override void Die()
    {
        // «√∑π¿ÃæÓ¿« ªÁ∏¡¿ª æÀ∏≤
    }
}
