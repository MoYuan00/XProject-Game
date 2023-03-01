using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_UI : MonoBehaviour
{
    /*****************UI主函数(一直显示在屏幕上的UI和其他UI的入口)******************/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickTest_UI()
    {
        Test_UI test = Manager.UI.Show<Test_UI>();
        test.SetTitle("梦鸢是白白的RBQ~");
        test.OnClose += Test_OnClose;//添加点击事件
    }

    private void Test_OnClose(Window_UI sender, Window_UI.WindowResult result)
    {
        Debug.Log("点击了对话框响应的结果：" + result);
    }
}
