using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Start()
    {
        //不可销毁,切换场景时也不会销毁
        DontDestroyOnLoad(this);

        //创建对象池
        Manager.Pool.CreateGameObjectPool("Shot",10);//子弹的对象池超过10秒不用的对象自动销毁

    }
}
