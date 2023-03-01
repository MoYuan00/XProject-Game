using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    /*****************声音管理器******************/

    //音乐
    AudioSource m_MusicAudio;
    //音效
    AudioSource m_SoundAudio;

    /// <summary>
    /// 音效音量
    /// </summary>
    private float SoundVolume
    {
        get { return PlayerPrefs.GetFloat("SoundVolume", 1.0f); }
        set
        {
            m_SoundAudio.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }

    /// <summary>
    /// 音乐音量
    /// </summary>
    private float MusicVolume
    {
        get { return PlayerPrefs.GetFloat("MusicVolume", 1.0f); }
        set
        {
            m_MusicAudio.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    private void Awake()
    {
        //音乐组件初始化
        m_MusicAudio = this.gameObject.AddComponent<AudioSource>();
        //自动播放关闭 如果上面有挂载音乐会自动播放,这个目前不需要自动播放
        m_MusicAudio.playOnAwake = false;
        //循环播放
        m_MusicAudio.loop = true;

        //音效组件初始化
        m_SoundAudio = this.gameObject.AddComponent<AudioSource>();
        //关闭循环 因为上面也没挂载音效是动态添加的所以自动播放开关效果都是关,所以没写因为多余
        m_SoundAudio.loop = false;
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusic(string name)
    {
        //性能考虑音量小于0.1就不播放了
        if (this.MusicVolume < 0.1f)
            return;
        string oldName = "";
        if (m_MusicAudio.clip != null)
            oldName = m_MusicAudio.clip.name;
        //相同音乐不重复加载
        if (oldName == name)
        {
            m_MusicAudio.Play();
            return;
        }
        //加载音乐并播放
        AudioClip audioClip = Resources.Load(name) as AudioClip;
        m_MusicAudio.clip = audioClip;
        m_MusicAudio.Play();
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        m_MusicAudio.Pause();
    }

    /// <summary>
    /// 继续播放
    /// </summary>
    public void OnUnPauseMusic()
    {
        m_MusicAudio.UnPause();
    }

    /// <summary>
    /// 停止音乐
    /// </summary>
    public void StopMusic()
    {
        m_MusicAudio.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name)
    {
        if (this.SoundVolume < 0.1f)
            return;
        //加载音效并播放
        AudioClip audioClip = Resources.Load(name) as AudioClip;
        m_SoundAudio.PlayOneShot(audioClip);
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="value"></param>
    public void SetMusicVolume(float value)
    {
        this.MusicVolume = value;
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="value"></param>
    public void SetSoundVolume(float value)
    {
        this.SoundVolume = value;
    }
}