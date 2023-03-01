using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    /*****************事件管理器******************/

    public delegate void EnventHandler(object args);
    //存所有事件的字典
    Dictionary<int, EnventHandler> m_Events = new Dictionary<int, EnventHandler>();

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="e"></param>
    public void Subscribe(int id, EnventHandler e)
    {
        //如果这个id已经订阅过事件就+=事件,多播委托
        if (m_Events.ContainsKey(id))
            m_Events[id] += e;
        else//如果是第一次订阅就存入一下集合
            m_Events.Add(id, e);
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="id"></param>
    /// <param name="e"></param>
    public void UnSubscribe(int id, EnventHandler e)
    {
        //如果字典包含才执行
        if (m_Events.ContainsKey(id))
        {
            //如果有事件就执行取消这个事件的订阅
            if (m_Events[id] != null)
                m_Events[id] -= e;
            //如果这个id对应的事件一个都没了,把当前id从字典中删除
            if (m_Events[id] == null)
                m_Events.Remove(id);
        }
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="args"></param>
    public void Fire(int id, object args = null)
    {
        EnventHandler handler;
        //如果存在要执行的事件
        if (m_Events.TryGetValue(id, out handler))
        {
            //执行 
            handler(args);
        }
    }
}