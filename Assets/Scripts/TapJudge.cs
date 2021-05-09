using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TapJudge : MonoBehaviour
{
    public Text Text1;
    public bool judge = false;
    public System.Diagnostics.Stopwatch judger = new System.Diagnostics.Stopwatch();
    public bool even = true;
    //public bool test = true;
    public bool stopperfect = false;
    public GameObject runProgrum;
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.AddComponent<>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(runProgrum.ToString() != "Run Programs")
        {
            runProgrum = GameObject.Find("Run Programs");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (judge == false)
        {
            judger.Start();
            judge = true;
            stopperfect = true;
            runProgrum.GetComponent<NoteJudge>().JudgeLine();
        }
        else
        {
            var time = judger.Elapsed.TotalSeconds;
            if (time < 0.1)
            {
                if (stopperfect)
                {
                    runProgrum.GetComponent<NoteJudge>().Perfect();
                    even = false;
                }
            }
            else if (time < 0.3)
            {
                runProgrum.GetComponent<NoteJudge>().Great();
                even = false;
            }
            else if (time < 0.5)
            {
                runProgrum.GetComponent<NoteJudge>().Good();
                even = false;
            }
            //test = false;
        }

    }

    public void Tap()
    {
        runProgrum.GetComponent<NoteJudge>().Tap();
        //if (test)
        //{
        if (judge == false)
        {
            judger.Start();
            judge = true;
            stopperfect = true;
        }
        else
        {
            var time = judger.Elapsed.TotalSeconds;
            if (time < 0.1)
            {
                if (stopperfect)
                {
                    runProgrum.GetComponent<NoteJudge>().Perfect();
                    even = false;
                }
            }
            else if (time < 0.3)
            {
                runProgrum.GetComponent<NoteJudge>().Great();
                even = false;
            }
            else if (time < 0.5)
            {
                runProgrum.GetComponent<NoteJudge>().Good();
                even = false;
            }
            //test = false;
        }
        //}
    }

    private void OnBecameInvisible()
    {
        if(even)
        {
            //runProgrum.GetComponent<NoteJudge>().Miss();
        }
    }
}
