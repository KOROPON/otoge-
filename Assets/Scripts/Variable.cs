using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Variable : MonoBehaviour
{
    public int score = 0;
    public static float speed = 4f;
    public Text text2;
    public static float beat = 4f;
    public static float bpm = 170f;
    public AudioSource music;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine("PlayMusic");
    }

    // Update is called once per frame
    void Update()
    {
        //score�\��
        // text2.text = score.ToString();
    }

    private IEnumerator PlayMusic()
    {
      Debug.Log(string.Format("before{0:N3}", Time.time));
      yield return new WaitForSeconds(5);
      Debug.Log(string.Format("after{0:N3}", Time.time));
      music.Play();
    }
}
