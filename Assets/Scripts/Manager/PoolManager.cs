using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //挂载对象池的父节点位置
    Transform m_PoolParent;

    //对象池字典
    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        //初始化获取父节点的位置
        m_PoolParent = this.transform.parent.Find("Pool");
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <typeparam name="T">对象池类型</typeparam>
    /// <param name="poolName">对象池名称</param>
    /// <param name="releaseTime">对象池自动释放时间/秒</param>
    private void CreatePool<T>(string poolName, float releaseTime)
        where T : PoolBase
    {//对象池字典不包含才执行创建对象池
        if (!m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            //实例化一个对象poolname
            GameObject go = new GameObject(poolName);
            //设置对象池父节点
            go.transform.SetParent(m_PoolParent);
            //创建对象池脚本挂载上去
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    /// <summary>
    /// 创建物体对象池
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="releaseTime"></param>
    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    /// <summary>
    /// 取出对象 传入要操作的对象池的名字 和要取出的对象名字
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public Object Spawn(string poolName, string assetName)
    {
        //从字典中找到该对象池
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            //传入要取出对象名字,取出对象
            return pool.Spwan(assetName);
        }
        return null;
    }

    /// <summary>
    /// 回收对象 要回收到的池子 对象名字 对象类型
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <param name="asset"></param>
    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
        {//回收对象
            pool.UnSpwan(assetName, asset);
        }
    }
}

