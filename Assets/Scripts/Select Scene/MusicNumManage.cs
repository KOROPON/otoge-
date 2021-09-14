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
using UnityEngine.Serialization;


public class MusicNumManage : MonoBehaviour
{
    Image _jack;
    private AudioSource _audioSource;
    private GetHighScores _getHighScores;
    private string _songName;
    private string _jacketPath;


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

    private void MusicInfo(string musicName,string jacketPath)
    {
        _jacketPath = jacketPath;
        _audioSource.clip = Resources.Load<AudioClip>(musicName);
        _audioSource.Play();
        _jack.sprite = Resources.Load<Sprite>(_jacketPath);
    }

    private void SelectSong(string musicName)
    {
        _jacketPath = "Jacket/" + musicName + "_jacket";
        MusicInfo("Songs/Music Select/" + musicName + "_intro", _jacketPath);
        title.text = musicName;
        PlayerPrefs.SetString("selected_song", musicName);
        _songName = musicName;
        highScore.text = $"{_getHighScores.GetHighScore(_songName, PlayerPrefs.GetString("difficulty")),9: 0,000,000}";
    }

    void Start()
    {
        _jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        _audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        scrollviewContent = GameObject.Find("Content");
        _getHighScores = FindObjectOfType<GetHighScores>();

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
        PlayerPrefs.SetString("difficulty", dif.name);
        difficulty = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, difficulty),9: 0,000,000}";

        for (int i = 0; i < scrollviewContent.transform.childCount; i++)
        {
            GameObject song = scrollviewContent.transform.GetChild(i).gameObject;
        }
    }
}
