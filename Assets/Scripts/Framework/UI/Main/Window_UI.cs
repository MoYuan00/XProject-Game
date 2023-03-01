using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Window_UI : MonoBehaviour
{
    /*****************所有UI的父类******************/

    public delegate void CloseHandler(Window_UI sender, WindowResult result);//事件
    public event CloseHandler OnClose;//注册点击事件

    public virtual System.Type Type { get { return this.GetType(); } }

    public enum WindowResult//定义一个枚举类型判断点击的是确定还是取消
    { 
    None=0,
    Yes,
    No
    }

    /// <summary>
    /// 关闭
    /// </summary>
    /// <param name="result"></param>
    public void Close(WindowResult result = WindowResult.None)
    {
        Manager.UI.Close(this.Type);//关闭UI
        if (this.OnClose != null)
            this.OnClose(this, result);//如果有点击事件就调用传入自己和选择的类型
        this.OnClose = null;//调用完清空事件
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void OnCloseClick()
    {
        this.Close();
    }
    /// <summary>
    /// 关闭但是选择的是Yes
    /// </summary>
    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }

    private void OnMouseDown()
    {
        Debug.LogFormat(this.name + "Clicked");
    }
}
