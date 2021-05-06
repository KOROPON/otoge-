using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoteJudge : MonoBehaviour
{
    public Text Text1;
    public int score;       //スコア
    public bool a = false;
    public bool b = false;
    public bool c = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //TapNote
    public void TapOn() {
        
    }

    //HoldNote
    public void HoldOn() {      //最初のタップ

    }
    public void HoldOut() {     //ずれるor離す

    }
    public void HoldIn() {      //戻ってきたとき

    }

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
        Text1.text = "Tap";
    }

    public void Tap()
    {
        Text1.text = "Miss...";
    }
    public void JudgeLine()
    {
        Text1.text = "touch judge line";
    }
}
