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
    private string _jacketPath;


    public int music_number = -1;
    public Text highScore;
    public Text title;
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

    private void MusicInfo(string music_name,string jacketPath)
    {
        _jacketPath = jacketPath;
        audioSource.clip = Resources.Load<AudioClip>(music_name);
        audioSource.Play();
        jack.sprite = Resources.Load<Sprite>(_jacketPath);
    }

    private void SelectSong(string musicname)
    {
        _jacketPath = "Jacket/" + musicname + "_jacket";
        MusicInfo("Songs/Music Select/" + musicname + "_intro", _jacketPath);
        title.text = musicname;
        PlayerPrefs.SetString("selected_song", musicname);
        songName = musicname;
    }

    void Start()
    {
        jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        scrollviewContent = GameObject.Find("Content");
        getHighScores = FindObjectOfType<GetHighScores>();

        if (!PlayerPrefs.HasKey("selected_song"))
        {
            PlayerPrefs.SetString("selected_song", "Collide");
        }

        if (!PlayerPrefs.HasKey("difficulty"))
        {
            PlayerPrefs.SetString("difficulty", "Easy");
        }

        SelectSong(PlayerPrefs.GetString("selected_song"));
        Difficulty(GetDifficulty(PlayerPrefs.GetString("difficulty")));
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
            SelectSong(obj.name);
        }
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
