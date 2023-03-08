using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    
    public Transform follow;
    void Update()
    {
        transform.position = follow.position;
    }
}
