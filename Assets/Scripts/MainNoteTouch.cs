using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainNoteTouch : MonoBehaviour
{
    //public TapJudge script;
    // Start is called before the first frame update
    void Start()
    {
        //script = GameObject.Find("TapBase").GetComponent<TapJudge>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.ToString() + this.gameObject.ToString());
        //if (other.ToString() == "JudgeLineJudgement")
        //{
    }*/
    private void OnTriggerEnter(Collider other)
    {
        var sc = transform.parent.gameObject.transform.Find("Base").gameObject.GetComponent<TapJudge>();
        sc.TrueJudge();
    }
}