using UnityEngine;
using UnityEngine.UI;

public class SettingField : MonoBehaviour
{
    private bool resume;
    public static bool SetBool;
    public Text text;
    public AudioSource aud;
    int resumetime = 180;

    [SerializeField]
    public GameObject pauseButton;
    public GameObject pausePanel;

    public void GamePause()
    {
        if (!SetBool)
        {
            return;
        }
        aud.Pause();
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        //text.text = "3";
        resume = true;
        //StartCoroutine("SettingRemuse");
    }

    private void Update()
    {
        if (resume)
        {
            resumetime--;
        }
        if (resumetime % 60 == 0)
        {
            text.text = (resumetime / 60).ToString();
            if (resumetime == 0)
            {
                pauseButton.SetActive(true);
                aud.UnPause();
                Time.timeScale = 1;
                resume = false;
                resumetime = 180;
            }
        }
    }

    public void Back()
    {
        Time.timeScale = 1;
        ChangeScene_PlayScene.playStopped = false;
        Shutter.blChange = "ToSFrP";
        Shutter.blShutterChange = "Close";
    }
    public void Retry()
    {
        Time.timeScale = 1;
        ChangeScene_PlayScene.playStopped = false;
        Shutter.blChange = "ToPFrP";
        Shutter.blShutterChange = "Close";
    }
}
