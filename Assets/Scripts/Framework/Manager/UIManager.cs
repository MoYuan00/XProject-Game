using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /*****************UI的数据类型******************/
    class UIElement
    {
        public string Resources;//路径
        public bool Cache;//静态还是动态  Cache=true 则是静态 反之动态 区别 静态关闭的时候是隐藏UI,动态是删除UI实例
        public GameObject Instance;//实例
    }

    public Transform parent;//UI生成出来后的父物体

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();//存储所有UI

    public UIManager()
    {
                             /*****************配置所有UI初始数据******************/
        this.UIResources.Add(typeof(Test_UI), new UIElement() { Resources = "UI/Test_UI", Cache = true });
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Show UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))//如果字典包含
        {
            UIElement info = this.UIResources[type];//拿到这个ui的数据
            if (info.Instance != null)//实例不等空直接显示
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);//加载UI
                if (prefab == null)//路径是错的
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);//实例化UI并保存数据
                info.Instance.transform.parent = parent;//设置父物体
                info.Instance.transform.localPosition = Vector3.zero;//设置坐标
                info.Instance.transform.localScale = Vector3.one;//设置坐标
            }
            return info.Instance.GetComponent<T>();//返回UI的脚本
        }
        return default(T);//字典不包含
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="type"></param>
    public void Close(Type type)
    {
        if (this.UIResources.ContainsKey(type))//如果字典包含是静态就隐藏一下,如果是动态就删除实例
        {
            UIElement info = this.UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        
        }
    }
   
  
}
