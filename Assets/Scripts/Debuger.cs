using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Debuger : MonoBehaviour
{
    public Text Text1;
    // Start is called before the first frame update
    void Start()
    {
        Text1.text = "pppppppppppppppppppppp";
    }

    
    // Update is called once per frame
    void Update()
    {

    }
    
    //TapNote
    public void TapOn() {
        
    }

    //HoldNote
    public void HoldOn() {      //�ŏ��̃^�b�v

    }
    public void HoldOut() {     //�����or����

    }
    public void HoldIn() {      //�߂��Ă����Ƃ�

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