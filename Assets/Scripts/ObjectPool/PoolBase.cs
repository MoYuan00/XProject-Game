using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池的基类
/// </summary>
public class PoolBase : MonoBehaviour
{
    //自动释放时间/秒
    protected float m_ReleaseTime;

    //上次释放资源的时间/毫微秒 1(秒)=10000000(毫微秒)
    protected long m_LastReleaseTime = 0;

    //真正的对象池
    protected List<PoolObject> m_Objects;

    public void Start()
    {
        //初始化的时候赋值一下时间为上一次释放资源时间
        m_LastReleaseTime = System.DateTime.Now.Ticks;
    }

    /// <summary>
    /// 初始化对象池传入自动释放时间/秒
    /// </summary>
    /// <param name="time"></param>
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects = new List<PoolObject>();
    }

    /// <summary>
    /// 取出对象
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
    /// 回收对象
    /// </summary>
    public virtual void UnSpwan(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);
        m_Objects.Add(po);
    }

    /// <summary>
    /// 释放子类自己重写=.=
    /// </summary>
    public virtual void Release()
    {

    }

    private void Update()
    {
        //如果上次释放资源的时间距离现在的时间,超过了自动释放的时间就释放资源  现在的时间减去上次释放的时间 >= 自动释放的时间
        if (System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            //把现在的时间再重新赋值给上次释放的时间
            m_LastReleaseTime = System.DateTime.Now.Ticks;
            //释放资源
            Release();
        }
    }
}
