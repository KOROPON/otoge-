using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Debuger : MonoBehaviour
{
    public Text Text1;
    public void Perfect()
    {
        Text1.text = "Perfect!!!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 1000;
    }
    public void Great()
    {
        Text1.text = "Great!!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 750;
    } 
    public void Good()
    {
        Text1.text = "good!";
        GameObject.Find("Run Programs").GetComponent<Variable>().score += 500;
    }
    public void Miss()
    {
        Text1.text = "Miss...";
    }

    public void Tap()
    {
        Text1.text = "Tap";
    }
    public void JudgeLine()
    {
        Text1.text = "touch judge line";
    }
}
