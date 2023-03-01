using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����صĻ���
/// </summary>
public class PoolBase : MonoBehaviour
{
    //�Զ��ͷ�ʱ��/��
    protected float m_ReleaseTime;

    //�ϴ��ͷ���Դ��ʱ��/��΢�� 1(��)=10000000(��΢��)
    protected long m_LastReleaseTime = 0;

    //�����Ķ����
    protected List<PoolObject> m_Objects;

    public void Start()
    {
        //��ʼ����ʱ��ֵһ��ʱ��Ϊ��һ���ͷ���Դʱ��
        m_LastReleaseTime = System.DateTime.Now.Ticks;
    }

    /// <summary>
    /// ��ʼ������ش����Զ��ͷ�ʱ��/��
    /// </summary>
    /// <param name="time"></param>
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects = new List<PoolObject>();
    }

    /// <summary>
    /// ȡ������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual Object Spwan(string name)
    {
        foreach (PoolObject po in m_Objects)
        {
            if (po.Name == name)
            {
                m_Objects.Remove(po);
                return po.Object;
            }
        }
        return null;
    }

    /// <summary>
    /// ���ն���
    /// </summary>
    public virtual void UnSpwan(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);
        m_Objects.Add(po);
    }

    /// <summary>
    /// �ͷ������Լ���д=.=
    /// </summary>
    public virtual void Release()
    {

    }

    private void Update()
    {
        //����ϴ��ͷ���Դ��ʱ��������ڵ�ʱ��,�������Զ��ͷŵ�ʱ����ͷ���Դ  ���ڵ�ʱ���ȥ�ϴ��ͷŵ�ʱ�� >= �Զ��ͷŵ�ʱ��
        if (System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            //�����ڵ�ʱ�������¸�ֵ���ϴ��ͷŵ�ʱ��
            m_LastReleaseTime = System.DateTime.Now.Ticks;
            //�ͷ���Դ
            Release();
        }
    }
}
