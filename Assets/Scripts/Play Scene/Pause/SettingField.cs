using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingField : MonoBehaviour
{
    bool resume;
    public Text text;
    public AudioSource aud;
    int resumetime = 180;

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

    public void Resume()
    {
        pausePanel.SetActive(false);
        countCanvas.SetActive(true);
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
                countCanvas.SetActive(false);
                pauseButton.SetActive(true);
                aud.UnPause();
                Time.timeScale = 1;
                resume = false;
                resumetime = 180;
            }
        }
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");
    }

    public void Back()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectScene");
    }
}
