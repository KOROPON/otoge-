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


    private void ensureSongPathExists(string songPath)
    {
    }

    private HighScores SongInfo(string songPath)
    {
        using (StreamReader reader = new StreamReader(songPath))
        {
            string jsonString = reader.ReadToEnd();
            return JsonUtility.FromJson<HighScores>(jsonString);
        }
    }


    void Start()
    {
        jack = GameObject.Find("ジャケット1").GetComponent<RawImage>();
        audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        //FindObjectOfType and set it as a private field
        getHighScores = FindObjectOfType<GetHighScores>();
        //Initialize Difficulty and Songname from user preferences or default values here

    }

    private void musicInfo(string music_name,string jacketPath,int score)
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
            //read the highscore here
            music_number = obj.transform.GetSiblingIndex();
            songName = obj.name;

        }
        musicInfo("Songs/Music Select/" + obj.name + "_intro", "Jacket/" + obj.name + "_jacket", 1000000);
    }

    public void Difficulty(GameObject dif)
    {
         difficulty = dif.name;

         highScore.text = String.Format("{0, 9: 0,000,000}", getHighScores.GetHighScore(songName, difficulty));
         Debug.Log(highScore);
         // Song songName = SongInfo("SongInformation.json").songs[];
         // string setDifficulty = difficulty switch
         // {
         //     "Easy" => highScore.text = songName.Easy.highScore.ToString(),
         //     "Hard" => highScore.text = songName.Hard.highScore.ToString(),
         //     "Extreme" => highScore.text = songName.Extreme.highScore.ToString(),
         //     "KUJO" => highScore.text = songName.KUJO.highScore.ToString(),
         //     _ => highScore.text = "",
         // };
    }
}
