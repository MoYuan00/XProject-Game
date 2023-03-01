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

    private static Main_UI _main_ui;
    public static Main_UI Main_ui
    {
        get { return _main_ui; }
    }

    private static UIManager _ui;
    public static UIManager UI
    {
        get { return _ui; }
    }

    private void Awake()
    {
        _pool = this.gameObject.AddComponent<PoolManager>();
        _sound = this.gameObject.AddComponent<SoundManager>();
        _event = this.gameObject.AddComponent<EventManager>();
        _main_ui = this.gameObject.AddComponent<Main_UI>();
        _ui = this.gameObject.GetComponent<UIManager>();
    }
}
