using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicNumManage : MonoBehaviour
{
    RawImage jack;
    public Text text1;
    public AudioSource audioSource;
    public int music_number;
    public AudioClip collide;
    public AudioClip devou;

    void Start()
    {
        jack = GameObject.Find("ジャケット1").GetComponent<RawImage>();
    }

    void musicInfo(string music_name,string musicPath,int score) {
        jack.texture = Resources.Load<Texture2D>(musicPath);
        text1.text = music_name;
    }

    public void Music1()
    {
        musicInfo("Collide","Music_Is_My_Suicide_Jacket",1000000);
        audioSource.clip = collide;
        audioSource.Play();
        if (music_number == 1)
        {
            SceneManager.LoadScene("PlayScene");
        }
        music_number = 1;
    }
    public void Music2() {
      musicInfo("Devourer Of Sol_Ⅲ","uchuu",1000000);
      audioSource.clip = devou;
      audioSource.Play();
      if (music_number == 2) {
        SceneManager.LoadScene("PlayScene");
      }
      music_number = 2;
    }
}
