using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : PoolBase
{
    /// <summary>
    /// 取出对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override Object Spwan(string name)
    {
        //没找到返回null
        Object obj = base.Spwan(name);
        if (obj == null)
            return null;
        //找到了返回GameObject类型,并且显示出来
        GameObject go = obj as GameObject;
        go.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public override void UnSpwan(string name, Object obj)
    {
        //隐藏GameObject 挂载位置移动到这个对象池上面 回收对象
        GameObject go = obj as GameObject;
        go.SetActive(false);
        go.transform.SetParent(this.transform, false);
        base.UnSpwan(name, obj);
    }

    /// <summary>
    /// 释放超过销毁时间的对象
    /// </summary>
    public override void Release()
    {
        base.Release();
        foreach (PoolObject item in m_Objects)
        {
            //现在时间减去具体每个对象上一次使用的时间,如果大于释放销毁时间,则执行销毁逻辑
            if (System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                Debug.Log("GameObjectPool release  time:" + System.DateTime.Now);
                Destroy(item.Object);//销毁对象物体
                m_Objects.Remove(item);//移除对象池
                Release();//再遍历自己一遍因为移除了一个list里面的数据,不重新遍历继续遍历会报错
                return;
            }
        }
    }
}