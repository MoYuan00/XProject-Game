using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Start()
    {
        //��������,�л�����ʱҲ��������
        DontDestroyOnLoad(this);

        //���������
        Manager.Pool.CreateGameObjectPool("Shot",10);//�ӵ��Ķ���س���10�벻�õĶ����Զ�����

    }
}
