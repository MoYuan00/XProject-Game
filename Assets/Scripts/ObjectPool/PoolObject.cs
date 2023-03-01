using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ش�ŵ���������
/// </summary>
public class PoolObject
{
    //�������
    public Object Object;

    //��������
    public string Name;

    //���һ��ʹ��ʱ��
    public System.DateTime LastUseTime;

    /// <summary>
    /// ����������ֺͶ���
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public PoolObject(string name, Object obj)
    {
        Name = name;
        Object = obj;
        //���һ��ʹ��ʱ���Ƕ��󴴽���ʱ���ʱ��
        LastUseTime = System.DateTime.Now;
    }
}
