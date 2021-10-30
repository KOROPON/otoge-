using Reilas;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shutter : MonoBehaviour
{
    [SerializeField] private GameObject jack;
    [SerializeField] private GameObject text;
    
    private Animator _anim;
    private static JudgeService _judgeService;
    private Image _jacket;
    private Text _title;

    public AudioSource openSe;
    public AudioSource closeSe;

    private static AudioSource _musicM;
    
    public static bool bltoPlay = false;
    public static string blChange;
    public static string blShutterChange;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
        jack.SetActive(true);
        text.SetActive(true);
        _jacket = jack.GetComponent<Image>();
        _title = text.GetComponent<Text>();
        jack.SetActive(false);
        text.SetActive(false);
        _musicM = gameObject.GetComponent<AudioSource>();
        _judgeService = gameObject.AddComponent<JudgeService>();

    }

    private void Update()
    {
        switch (blShutterChange)
        {
            case "Open":
                _anim.SetBool("bl", false);
                _anim.SetBool("blToPlay", false);
                break;
            case "Close":
                _anim.SetBool("bl", true);
                break;
            case "CloseToPlay":
                jack.SetActive(true);
                text.SetActive(true);
                _jacket.sprite = Resources.Load<Sprite>("Jacket/" + RhythmGamePresenter.musicName + "_jacket");
                _title.text = RhythmGamePresenter.musicName;
                _anim.SetBool("blToPlay", true);
                break;
        }
    }

    private void CloseFunction()
    {
        switch (blChange)
        {
            case "ToPFrP": 
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                AllNoteDestroy();
                SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
                break;

            case "ToSFrR":
                SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("ResultScene", UnloadSceneOptions.None);
                break;
            case "ToPFrR":
                SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("ResultScene", UnloadSceneOptions.None);
                break;
            case "ToSFrP":
                SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
                AllNoteDestroy();
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                break;
            case "ToR":
                SceneManager.LoadScene("ResultScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                break;
            case "ToS_F":
                SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("Title Scene", UnloadSceneOptions.None);
                break;
        }
    }

    private void CloseToPlayFunction()
    {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("SelectScene", UnloadSceneOptions.None);
    }

    private void PlaySongAudio()
    {
        if (bltoPlay)
        {
            Invoke("PlayAudio", 1f);
            bltoPlay = false;
        }
    }

    private void OpenAudio()
    {
        openSe.Play();
    }

    private void CloseAudio()
    {
        closeSe.Play();
    }


    private void PlayAudio()
    {
        RhythmGamePresenter.PlaySongs();
        Debug.Log("SongPlay");
        ChangeScenePlayScene.playNoticed = true;
        SettingField.setBool = true;
    }
    
    private static void AllNoteDestroy()
    {

        foreach (var a in RhythmGamePresenter.TapNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter.HoldNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter.AboveTapNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter.AboveSlideNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter.AboveHoldNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter.AboveChainNotes)
        {
            Destroy(a.gameObject);
        }

        foreach (var lane in RhythmGamePresenter.TapNoteLanes) lane.Clear();
        for (var i = 0; i < _judgeService.tapJudgeStartIndex.Length; i++)
        {
            
        }
            RhythmGamePresenter.internalNotes.Clear();
        RhythmGamePresenter.chainNotes.Clear();
        RhythmGamePresenter.TapNotes.Clear();
        RhythmGamePresenter.HoldNotes.Clear();
        RhythmGamePresenter.AboveTapNotes.Clear();
        RhythmGamePresenter.AboveSlideNotes.Clear();
        RhythmGamePresenter.AboveHoldNotes.Clear();
        RhythmGamePresenter.AboveChainNotes.Clear();
        RhythmGamePresenter.BarLines.Clear();
        BarLine.BarLines.Clear();
    }
}
