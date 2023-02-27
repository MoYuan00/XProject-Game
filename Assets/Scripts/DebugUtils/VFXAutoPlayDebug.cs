using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class VFXAutoPlayDebug : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    /// <summary>
    /// 上一次系统时间
    /// </summary>
    private  double m_PreviousTime;
    
    /// <summary>
    /// 当前运行时间
    /// </summary>
    private float _runningTime;
    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Play();
    }

    public void Update()
    {

        _runningTime += Time.deltaTime;
        // UpdateVFX();
    }


    private void UpdateVFX()
    {
        // Debug.Log(_runningTime);
        // _particleSystem.Simulate(_runningTime); // 更新播放
        // if (!_particleSystem.isPlaying)  // 如果粒子系统没有播放
        // {
            // _particleSystem.Play();  // 播放粒子系统
        // }
    }
}
