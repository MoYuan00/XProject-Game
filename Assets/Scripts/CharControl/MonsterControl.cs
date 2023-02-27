using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanHit))]
public class MonsterControl : MonoBehaviour
{
    private CanHit _canHit;
    public void Awake()
    {
        _canHit = GetComponent<CanHit>();
        _canHit.onHitEvent.AddListener(OnHit);
    }

    public void OnHit(Bullet bullet)
    {
        Debug.Log("被攻击！！！");
    }
}

