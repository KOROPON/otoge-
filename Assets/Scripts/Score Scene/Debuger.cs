using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Debuger : MonoBehaviour
{
    public Text text2;
    public void Perfect()
    {
        text2.text = "Perfect!!!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 1000;
    }
    public void Great()
    {
        text2.text = "Great!!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 750;
    } 
    public void Good()
    {
        text2.text = "good!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 500;
    }
    public void Miss()
    {
        if (text2)
        {
            text2.text = "Miss...";
        }
    }

}
