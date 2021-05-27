using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
public class HoldMain : MonoBehaviour
{
    bool count = false;
    bool roop = false;
    bool a = false;
    public Debuger script;
    private IEnumerator holdJudge;
    void Start()
    {
        script = GameObject.Find("Run Programs").GetComponent<Debuger>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (count == false)
        {
            var cs = transform.parent.gameObject.transform.Find("Base").gameObject.GetComponent<HoldJudge>();
            cs.TrueJudge();
            IEnumerator holdJudge = TestCoroutine();
            StartCoroutine(holdJudge);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        roop = true;
    }
    public void Enter()
    {
        a = true;
    }
    public void Exit()
    {
        a = false;
    }
    private IEnumerator TestCoroutine()
    {
        while(!roop)
        {
            yield return new WaitForSeconds(0.5f);
            if (a == true && !roop)
            {
                Debug.Log("Perfect");
                script.Perfect();
            }
            else if(!roop)
            {
                Debug.Log("Miss...");
                script.Miss();
            }
        }
    }
}
