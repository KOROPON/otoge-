using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TapJudge : MonoBehaviour, IPointerClickHandler
{
    public Text Text1;
    public bool judge;
    public System.Diagnostics.Stopwatch judger = new System.Diagnostics.Stopwatch();
    public bool even = false;
    public bool onlytap = true;
    //public bool stopperfect = false;
    public GameObject runProgrum;
    public Debuger script;
    public int a = 0;
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.AddComponent<>(); 
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
        judge = false;
        even = false;
    }
    public void OnPointerClick(PointerEventData eventData)   //�^�b�v���ꂽ�Ƃ�
    {
        //double time = judger.Elapsed.TotalSeconds;
        if (even == false)
        {
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
                        even = true;
                    }
                    else if (time < 0.3)
                    {
                        script.Great();
                        Debug.Log("Great2");
                        even = true;
                    }
                    else if (time < 0.5)
                    {
                        script.Good();
                        Debug.Log("Good2");
                        even = true;
                    }
                    //test = false;
                }
                //}
                onlytap = false;
            }
        }
    }
    public void TrueJudge()          //�^�񒆂����C���ɐG�ꂽ�Ƃ�
    {
        if (even == false)
        {
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
                    even = true;
                }
                else if (time < 0.3)
                {
                    script.Great();
                    Debug.Log("Great1");
                    even = true;
                }
                else if (time < 0.5)
                {
                    script.Good();
                    Debug.Log("Good1");
                    even = true;
                }
            }
        }
    }
    // Update is called once per frame
    /*void Update()
    {
        if(runProgrum.ToString() != "Run Programs")
        {
            runProgrum = GameObject.Find("Run Programs");

        }
        if(Text1 == null)
        {
            Text1 = GameObject.Find("Text1").GetComponent<Text>();
            //Text1 = Text.FindObjectOfType<Text>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.ToString() == "JudgeLineVisual")
        {
            Debug.Log(other);
        }
    }*/

    public void OnBecameInvisible()   //�J�������猩���Ȃ��Ȃ����Ƃ�
    {
        if(even == false)
        {
            //RunPrograms.GetComponent<NoteJudge>().Miss();
            Debuger script = GameObject.Find("Run Programs").GetComponent<Debuger>();
            script.Miss();
            Debug.Log("Miss...");
        }
        Destroy(transform.root.gameObject);
    }
}
