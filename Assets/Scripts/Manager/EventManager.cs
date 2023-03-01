using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    /*****************�¼�������******************/

    public delegate void EnventHandler(object args);
    //�������¼����ֵ�
    Dictionary<int, EnventHandler> m_Events = new Dictionary<int, EnventHandler>();

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="id"></param>
    /// <param name="e"></param>
    public void Subscribe(int id, EnventHandler e)
    {
        //������id�Ѿ����Ĺ��¼���+=�¼�,�ಥί��
        if (m_Events.ContainsKey(id))
            m_Events[id] += e;
        else//����ǵ�һ�ζ��ľʹ���һ�¼���
            m_Events.Add(id, e);
    }

    /// <summary>
    /// ȡ������
    /// </summary>
    /// <param name="id"></param>
    /// <param name="e"></param>
    public void UnSubscribe(int id, EnventHandler e)
    {
        //����ֵ������ִ��
        if (m_Events.ContainsKey(id))
        {
            //������¼���ִ��ȡ������¼��Ķ���
            if (m_Events[id] != null)
                m_Events[id] -= e;
            //������id��Ӧ���¼�һ����û��,�ѵ�ǰid���ֵ���ɾ��
            if (m_Events[id] == null)
                m_Events.Remove(id);
        }
    }

    /// <summary>
    /// ִ���¼�
    /// </summary>
    /// <param name="id"></param>
    /// <param name="args"></param>
    public void Fire(int id, object args = null)
    {
        EnventHandler handler;
        //�������Ҫִ�е��¼�
        if (m_Events.TryGetValue(id, out handler))
        {
            //ִ�� 
            handler(args);
        }
    }
}