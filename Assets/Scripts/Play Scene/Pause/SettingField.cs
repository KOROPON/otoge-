using UnityEngine;
using UnityEngine.UI;

public class SettingField : MonoBehaviour
{
    private bool _resume;
    public static bool setBool;
    public Text text;
    public AudioSource aud;
    private Image _fader;
    int _resumetime = 180;
    private BossGimmicks _boss;
    private RhythmGamePresenter _presenter;

    void Start()
    {
        _fader = GameObject.Find("Fader").GetComponent<Image>();
        _boss = GameObject.Find("BossGimmick").GetComponent<BossGimmicks>();
        _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }
    [SerializeField]
    public GameObject pauseButton;
    public GameObject pausePanel;

    public void GamePause()
    {
        if (!setBool)
        {
            return;
        }
        aud.Pause();
        _presenter.judgeForgive = false;
        Time.timeScale = 0;
        _boss.gimmickPause = true;
        _fader.enabled = true;
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        text.gameObject.SetActive(true);
        text.text = "3";
        _resume = true;
    }

    private void Update()
    {
        if (!_resume) return;
        
        _resumetime--;
        
        if (_resumetime % 60 == 0)
        {
            text.text = (_resumetime / 60).ToString();
            if (_resumetime == 0)
            {
                _fader.enabled = false;
                pauseButton.SetActive(true);
                text.gameObject.SetActive(false);
                _presenter.judgeForgive = true;
                aud.UnPause();
                Time.timeScale = 1;
                _boss.gimmickPause = false;
                _resume = false;
                _resumetime = 180;
            }
        }
    }

    public void Back()
    {
        Time.timeScale = 1;
        ChangeScenePlayScene.playStopped = false;
        setBool = false;
        Shutter.blChange = "ToSFrP";
        Shutter.blShutterChange = "Close";
    }
    public void Retry()
    {
        Time.timeScale = 1;
        ChangeScenePlayScene.playStopped = false;
        setBool = false;
        Shutter.blChange = "ToPFrP";
        Shutter.blShutterChange = "Close";
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GamePause();
        }
    }
}
