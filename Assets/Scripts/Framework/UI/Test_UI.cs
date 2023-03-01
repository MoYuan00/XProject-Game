using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_UI : Window_UI
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTitle(string text)
    {
        this.text.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
