using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Variable : MonoBehaviour
{
    public int score = 0;
    public float speed = 1f;
    public Text text2;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        //score�\��
        text2.text = score.ToString();
    }
}