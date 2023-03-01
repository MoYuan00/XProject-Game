using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : PoolBase
{
    /// <summary>
    /// ȡ������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override Object Spwan(string name)
    {
        //û�ҵ�����null
        Object obj = base.Spwan(name);
        if (obj == null)
            return null;
        //�ҵ��˷���GameObject����,������ʾ����
        GameObject go = obj as GameObject;
        go.SetActive(true);
        return obj;
    }

    /// <summary>
    /// ���ն���
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public override void UnSpwan(string name, Object obj)
    {
        //����GameObject ����λ���ƶ��������������� ���ն���
        GameObject go = obj as GameObject;
        go.SetActive(false);
        go.transform.SetParent(this.transform, false);
        base.UnSpwan(name, obj);
    }

    /// <summary>
    /// �ͷų�������ʱ��Ķ���
    /// </summary>
    public override void Release()
    {
        base.Release();
        foreach (PoolObject item in m_Objects)
        {
            //����ʱ���ȥ����ÿ��������һ��ʹ�õ�ʱ��,��������ͷ�����ʱ��,��ִ�������߼�
            if (System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                Debug.Log("GameObjectPool release  time:" + System.DateTime.Now);
                Destroy(item.Object);//���ٶ�������
                m_Objects.Remove(item);//�Ƴ������
                Release();//�ٱ����Լ�һ����Ϊ�Ƴ���һ��list���������,�����±������������ᱨ��
                return;
            }
        }
    }
}