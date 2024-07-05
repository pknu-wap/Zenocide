using UnityEngine;
using UnityEngine.Pool;

public class Poolable : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }
}