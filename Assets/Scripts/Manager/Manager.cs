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

    private static SoundManager _sound;
    public static SoundManager Sound
    {
        get { return _sound; }
    }

    private static EventManager _event;
    public static EventManager Event
    {
        get { return _event; }
    }

    private void Awake()
    {
        _pool = this.gameObject.AddComponent<PoolManager>();
        _sound = this.gameObject.AddComponent<SoundManager>();
        _event = this.gameObject.AddComponent<EventManager>();
    }
}
