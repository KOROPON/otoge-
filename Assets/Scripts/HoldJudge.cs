using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Timers;
public class HoldJudge : MonoBehaviour
{
    Timer timer = new Timer(300);
    public Text Text1;
    public bool judge;
    public System.Diagnostics.Stopwatch judger = new System.Diagnostics.Stopwatch();
    public bool miss = true;
    public bool even = false;
    public bool onlytap = true;
    //public bool stopperfect = false;
    public GameObject runProgrum;
    public Debuger script;
    public int a = 0;
    public HoldMain cs;
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.AddComponent<>(); 
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
        judge = false;
        miss = true;
        cs = transform.parent.gameObject.transform.Find("HoldNote2").gameObject.GetComponent<HoldMain>();
    }
    void Update()
    {
        double time = judger.Elapsed.TotalSeconds;
        if (time > 0.4 && miss)
        {
            miss = false;
            if (!even)
            {
                Debug.Log("miss...");
            }
        }
    }
    public void OnMouseDown()   //タップされたとき
    {
        Debug.Log("タップをされたよ");
        even = true;
        //runProgrum.GetComponent<NoteJudge>().Tap();
        //script.Tap();
        if (onlytap)
        {
            //{
            if (!judge)
            {
                judge = true;
                judger.Start();//stopWatch
                a++;
            }
            else
            {
                var time = judger.Elapsed.TotalSeconds;
                if (time < 0.1)
                {
                    script.Perfect();
                    Debug.Log("Perfect2");

                }
                else if (time < 0.25)
                {
                    script.Great();
                    Debug.Log("Great2");
                }
                else if (time < 0.4)
                {
                    script.Good();
                    Debug.Log("Good2");
                }
                //test = false;
            }
            //}
            onlytap = false;
        }
    }
    public void TrueJudge()          //真ん中がラインに触れたとき
    {
        Debug.Log("真ん中触れたよ");
        if (!judge)
        {
            judger.Start();
            judge = true;
        }
        else
        {
            double time = judger.Elapsed.TotalSeconds;
            if (time < 0.1)
            {
                script.Perfect();
                Debug.Log("Perfect1");
            }
            else if (time < 0.25)
            {
                script.Great();
                Debug.Log("Great1");
            }
            else if (time < 0.4)
            {
                script.Good();
                Debug.Log("Good1");
            }
        }
    }
    public void OnBecameInvisible()   //カメラから見えなくなったとき
    {
        Destroy(transform.root.gameObject);
    }

    public void OnMouseEnter()
    {
        cs.Enter();
    }
    public void OnMouseExit()
    {
        cs.Exit();
    }
}
