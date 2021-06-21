using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class TapMain : MonoBehaviour
{
    private void OnTriggerEnter()
    {
        var sc = transform.parent.gameObject.transform.Find("Base").gameObject.GetComponent<TapJudge>();
        sc.TrueJudge();
    }
}