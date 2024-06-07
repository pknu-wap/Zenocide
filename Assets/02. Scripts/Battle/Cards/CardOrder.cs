// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrder : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] string sortingLayerName;
    int[] originOrders;
    int originOrder;

    private void Awake()
    {
        // 각 렌더러들의 order를 미리 저장해둔다.
        originOrders = new int[renderers.Length];

        for (int i = 0; i < originOrders.Length; ++i)
        {
            originOrders[i] = renderers[i].sortingOrder;
        }
    }

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }

    public void SetMiddleFrontOrder()
    {
        SetOrder(50);
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].sortingLayerName = sortingLayerName;
            renderers[i].sortingOrder = mulOrder + originOrders[i];
        }
    }
}