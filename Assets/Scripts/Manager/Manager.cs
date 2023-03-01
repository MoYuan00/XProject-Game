using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static PoolManager _pool;
    public static PoolManager Pool
    {
        get { return _pool; }
    }

    private void Awake()
    {
        _pool = this.gameObject.AddComponent<PoolManager>();
    }
}
