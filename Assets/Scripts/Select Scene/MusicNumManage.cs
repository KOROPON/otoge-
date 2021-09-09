using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Reilas;


public class MusicNumManage : MonoBehaviour
{
  RawImage jack;
  private AudioSource audioSource;
  private GetHighScores getHighScores;
  private string songName;
  public int music_number = -1;
  public Text highScore;

  public static string difficulty;

    void Start()
    {
        jack = GameObject.Find("ジャケット1").GetComponent<RawImage>();
        audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        getHighScores = FindObjectOfType<GetHighScores>();
        music_number = -1;
    }

    private void MusicInfo(string music_name,string jacketPath,float score)
    {
        audioSource.clip = Resources.Load<AudioClip>(music_name);
        audioSource.Play();
        jack.texture = Resources.Load<Texture2D>(jacketPath);
    }

    public void Tap(GameObject obj)
    {
        Debug.Log(obj.name + "_intro");
        if(music_number == obj.transform.GetSiblingIndex())
        {
            RhythmGamePresenter.musicname = obj.name;
            SceneManager.LoadScene("PlayScene");
        }
        else
        {
            music_number = obj.transform.GetSiblingIndex();
            songName = obj.name;
        }
        MusicInfo("Songs/Music Select/" + obj.name + "_intro", "Jacket/" + obj.name + "_jacket", 1000000);
    }

    public void Difficulty(GameObject dif)
    {
         difficulty = dif.name;
         highScore.text = String.Format("{0, 9: 0,000,000}", getHighScores.GetHighScore(songName, difficulty));
         Debug.Log(highScore.text);
    }
}
