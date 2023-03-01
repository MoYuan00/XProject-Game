using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //���ض���صĸ��ڵ�λ��
    Transform m_PoolParent;

    //������ֵ�
    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        //��ʼ����ȡ���ڵ��λ��
        m_PoolParent = this.transform.parent.Find("Pool");
    }

    /// <summary>
    /// ���������
    /// </summary>
    /// <typeparam name="T">���������</typeparam>
    /// <param name="poolName">���������</param>
    /// <param name="releaseTime">������Զ��ͷ�ʱ��/��</param>
    private void CreatePool<T>(string poolName, float releaseTime)
        where T : PoolBase
    {//������ֵ䲻������ִ�д��������
        if (!m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            //ʵ����һ������poolname
            GameObject go = new GameObject(poolName);
            //���ö���ظ��ڵ�
            go.transform.SetParent(m_PoolParent);
            //��������ؽű�������ȥ
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="releaseTime"></param>
    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    /// <summary>
    /// ȡ������ ����Ҫ�����Ķ���ص����� ��Ҫȡ���Ķ�������
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public Object Spawn(string poolName, string assetName)
    {
        //���ֵ����ҵ��ö����
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            //����Ҫȡ����������,ȡ������
            return pool.Spwan(assetName);
        }
        return null;
    }

    /// <summary>
    /// ���ն��� Ҫ���յ��ĳ��� �������� ��������
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <param name="asset"></param>
    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
        {//���ն���
            pool.UnSpwan(assetName, asset);
        }
    }
}

