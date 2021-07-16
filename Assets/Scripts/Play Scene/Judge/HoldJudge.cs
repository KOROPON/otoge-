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
    public bool judge1 = false;
    public bool judge2 = false;
    public System.Diagnostics.Stopwatch judger = new System.Diagnostics.Stopwatch();
    public System.Diagnostics.Stopwatch lineOver = new System.Diagnostics.Stopwatch();
    public bool miss = true;
    public bool even = false;
    public bool onlytap = true;
    //public bool stopperfect = false;
    public GameObject runProgrum;
    public Debuger script;
    public HoldMain cs;
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.AddComponent<>();
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
        miss = true;
        cs = transform.parent.gameObject.transform.Find("HoldNote2").gameObject.GetComponent<HoldMain>();
    }
    void Update()
    {
        double time = judger.Elapsed.TotalSeconds;
        if (time > 0.4 && miss &&lineOver.Elapsed.TotalSeconds > 0.4)
        {
            miss = false;
            if (!even)
            {
                Debug.Log("miss...");
                script.Miss();
            }
        }
    }
    public void OnMouseDown()   //�^�b�v���ꂽ�Ƃ�
    {
        if (onlytap)
        {
            //if (!judge1)
            //{
                judge1 = true;
                judger.Restart();//stopWatch
            //}
            if(judge2)
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
                even = true;
            }
            onlytap = false;
        }
    }
    public void TrueJudge()          //�^�񒆂����C���ɐG�ꂽ�Ƃ�
    {
        lineOver.Start();
        //if (!judge2)
        //{
            judger.Start();
            judge2 = true;
        //}
        if(judge1)
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
            even = true;
        }
    }
    public void OnBecameInvisible()   //�J�������猩���Ȃ��Ȃ����Ƃ�
    {
        Destroy(transform.parent.gameObject);
        Debug.Log("Lost");
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
