using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class Variable : MonoBehaviour
{
    public int score = 0;
    public static float speed = 1f;
    public Text text2;
    public static float beat = 4f;
    public static float bpm = 170f;
    public AudioSource music;

//    NoteSpawner sc;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
      //  sc = GameObject.Find("Note").GetComponent<NoteSpawner>();
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
        yield return new WaitForSeconds(3);
    //    sc.wait = true;
        music.Play();
    }
}
