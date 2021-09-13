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
    Image jack;
    private AudioSource audioSource;
    private GetHighScores getHighScores;
    private string songName;
    public int music_number = -1;
    public Text highScore;
    public GameObject scrollviewContent;

    public static string difficulty;

    private GameObject GetDifficulty(string diff)
    {
        GameObject getDiff = diff switch
        {
            "Easy" => GameObject.Find("Easy"),
            "Hard" => GameObject.Find("Hard"),
            "Extreme" => GameObject.Find("Extreme"),
            "KUJO" => GameObject.Find("KUJO"),
            _ => null
        };
        return getDiff;
    }

    void Start()
    {
        jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        scrollviewContent = GameObject.Find("Content");
        getHighScores = FindObjectOfType<GetHighScores>();
        music_number = -1;

        if (!PlayerPrefs.HasKey("selected_song"))
        {
            PlayerPrefs.SetString("selectedSong", "Collide");
        }

        if (!PlayerPrefs.HasKey("difficulty"))
        {
            PlayerPrefs.SetString("difficulty", "Easy");
        }

        Difficulty(GetDifficulty(PlayerPrefs.GetString("difficulty")));
    }

    private void MusicInfo(string music_name,string jacketPath)
    {
        audioSource.clip = Resources.Load<AudioClip>(music_name);
        audioSource.Play();
        jack.sprite = Resources.Load<Sprite>(jacketPath);
    }

    public void Tap(GameObject obj)
    {
        if (PlayerPrefs.GetString("selected_song") == obj.name)
        {
            RhythmGamePresenter.musicname = obj.name;
            SceneManager.LoadScene("PlayScene");
        }
        else
        {
            PlayerPrefs.SetString("selected_song", obj.name);
            songName = obj.name;
        }
        MusicInfo("Songs/Music Select/" + obj.name + "_intro", "Jacket/" + obj.name + "_jacket");
    }

    public void Difficulty(GameObject dif)
    {
        difficulty = dif.name;
        highScore.text = String.Format("{0, 9: 0,000,000}", getHighScores.GetHighScore(songName, difficulty));

        for (int i = 0; i < scrollviewContent.transform.childCount; i++)
        {
            GameObject song = scrollviewContent.transform.GetChild(i).gameObject;
        }
    }
}
