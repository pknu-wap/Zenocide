// ±ËπŒ√∂
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    #region ΩÃ±€≈Ê
    public static Player Instance { get; set; }
    private static Player instance;

    public override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion ΩÃ±€≈Ê

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
        BattleManager.Instance.GameOver();
    }
}
