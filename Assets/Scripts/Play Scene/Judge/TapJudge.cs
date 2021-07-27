using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TapJudge : MonoBehaviour, IPointerClickHandler
{
    public Text Text1;
    public AudioSource aud;
    public AudioClip audC;
    public bool judge;
    public System.Diagnostics.Stopwatch judger = new System.Diagnostics.Stopwatch();
    public bool even = false;
    public bool onlytap = true;
    //public bool stopperfect = false;
    public GameObject runProgrum;
    public Debuger script;
    public int a = 0;


    void Start()
    {
        //this.gameObject.AddComponent<>();
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
        judge = false;
        even = false;
        aud = GameObject.Find("TapMusic").GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData)   //�^�b�v���ꂽ�Ƃ�
    {
        //double time = judger.Elapsed.TotalSeconds;
        if (!even)
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
                    even = true;
                    if (time < 0.1)
                    {
                        script.Perfect();
                        Debug.Log("Perfect2");
                    }
                    else if (time < 0.3)
                    {
                        script.Great();
                        Debug.Log("Great2");
                    }
                    else if (time < 0.5)
                    {
                        script.Good();
                        Debug.Log("Good2");
                    }
                    else
                    {
                        script.Miss();
                        Debug.Log("Miss...");
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
        if (!even)
        {
            if (!judge)
            {
                judger.Start();
                judge = true;
            }
            else
            {
                double time = judger.Elapsed.TotalSeconds;
                even = true;
                if (time < 0.1)
                {
                    script.Perfect();
                    Debug.Log("Perfect1");
                }
                else if (time < 0.3)
                {
                    script.Great();
                    Debug.Log("Great1");
                }
                else if (time < 0.5)
                {
                    script.Good();
                    Debug.Log("Good1");
                }
                else
                {
                    script.Miss();
                    Debug.Log("Miss...");
                }
            }
        }
    }

    public void OnBecameInvisible()   //�J�������猩���Ȃ��Ȃ����Ƃ�
    {
        if(!even)
        {
            script.Miss();
            Debug.Log("Miss...");

        }
        Destroy(transform.parent.gameObject);
    }
}
