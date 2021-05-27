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
    }
    public void Great()
    {
        Text1.text = "Great!!";
    } 
    public void Good()
    {
        Text1.text = "good!";

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
