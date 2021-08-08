using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingField : MonoBehaviour
{
    bool remuse;
    public Text text;
    public AudioSource aud;
    int remusetime = 180;

    [SerializeField]
    public GameObject pauseButton;
    public GameObject pausePanel;
    public GameObject countCanvas;

    public void GamePause()
    {
        aud.Pause();
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Remuse()
    {
        pausePanel.SetActive(false);
        countCanvas.SetActive(true);
        //text.text = "3";
        remuse = true;
        //StartCoroutine("SettingRemuse");
    }

    private void Update()
    {
        if (remuse)
        {
            remusetime--;
        }
        if(remusetime % 60 == 0)
        {
            text.text = (remusetime / 60).ToString();
            if(remusetime == 0)
            {
                countCanvas.SetActive(false);
                pauseButton.SetActive(true);
                aud.UnPause();
                Time.timeScale = 1;
                remuse = false;
                remusetime = 180;
            }
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void Back()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
