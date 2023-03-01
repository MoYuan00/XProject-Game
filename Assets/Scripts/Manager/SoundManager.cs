using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    /*****************����������******************/

    //����
    AudioSource m_MusicAudio;
    //��Ч
    AudioSource m_SoundAudio;

    /// <summary>
    /// ��Ч����
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
    /// ��������
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
        //���������ʼ��
        m_MusicAudio = this.gameObject.AddComponent<AudioSource>();
        //�Զ����Źر� ��������й������ֻ��Զ�����,���Ŀǰ����Ҫ�Զ�����
        m_MusicAudio.playOnAwake = false;
        //ѭ������
        m_MusicAudio.loop = true;

        //��Ч�����ʼ��
        m_SoundAudio = this.gameObject.AddComponent<AudioSource>();
        //�ر�ѭ�� ��Ϊ����Ҳû������Ч�Ƕ�̬��ӵ������Զ����ſ���Ч�����ǹ�,����ûд��Ϊ����
        m_SoundAudio.loop = false;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusic(string name)
    {
        //���ܿ�������С��0.1�Ͳ�������
        if (this.MusicVolume < 0.1f)
            return;
        string oldName = "";
        if (m_MusicAudio.clip != null)
            oldName = m_MusicAudio.clip.name;
        //��ͬ���ֲ��ظ�����
        if (oldName == name)
        {
            m_MusicAudio.Play();
            return;
        }
        //�������ֲ�����
        AudioClip audioClip = Resources.Load(name) as AudioClip;
        m_MusicAudio.clip = audioClip;
        m_MusicAudio.Play();
    }

    /// <summary>
    /// ��ͣ����
    /// </summary>
    public void PauseMusic()
    {
        m_MusicAudio.Pause();
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void OnUnPauseMusic()
    {
        m_MusicAudio.UnPause();
    }

    /// <summary>
    /// ֹͣ����
    /// </summary>
    public void StopMusic()
    {
        m_MusicAudio.Stop();
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    public void PlaySound(string name)
    {
        if (this.SoundVolume < 0.1f)
            return;
        //������Ч������
        AudioClip audioClip = Resources.Load(name) as AudioClip;
        m_SoundAudio.PlayOneShot(audioClip);
    }

    /// <summary>
    /// ���ñ�����������
    /// </summary>
    /// <param name="value"></param>
    public void SetMusicVolume(float value)
    {
        this.MusicVolume = value;
    }

    /// <summary>
    /// ������Ч����
    /// </summary>
    /// <param name="value"></param>
    public void SetSoundVolume(float value)
    {
        this.SoundVolume = value;
    }
}