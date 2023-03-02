using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSpeed : MonoBehaviour
{
    public Text _text;
    public Rigidbody rb;

    private void Update()
    {
        var velocity = rb.velocity;
        
        var h = new Vector2(velocity.x, velocity.z).magnitude;
        _text.text = "水平:" + h.ToString("0.00") + " 垂直:" + velocity.y.ToString("0.00");   
    }
}
