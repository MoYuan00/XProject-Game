using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池存放的数据类型
/// </summary>
public class PoolObject
{
    //具体对象
    public Object Object;

    //对象名字
    public string Name;

    //最后一次使用时间
    public System.DateTime LastUseTime;

    /// <summary>
    /// 传入对象名字和对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public PoolObject(string name, Object obj)
    {
        Name = name;
        Object = obj;
        //最后一次使用时间是对象创建的时候的时间
        LastUseTime = System.DateTime.Now;
    }
}
