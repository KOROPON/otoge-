using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class TapMain : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sc = transform.parent.gameObject.transform.Find("Base").gameObject.GetComponent<TapJudge>();
        sc.TrueJudge();
    }
}