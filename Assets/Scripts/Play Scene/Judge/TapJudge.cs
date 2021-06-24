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
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.AddComponent<>(); 
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
        judge = false;
        even = false;
        aud = GameObject.Find("TapMusic").GetComponent<AudioSource>();
    }
    public void OnPointerClick(PointerEventData eventData)   //タップされたとき
    {
        aud.Play();
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
    public void TrueJudge()          //真ん中がラインに触れたとき
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
    public void OnBecameInvisible()   //カメラから見えなくなったとき
    {
        if(even == false)
        {
            script.Miss();
            Debug.Log("Miss...");

        }
        Destroy(transform.parent.gameObject);
    }
}
