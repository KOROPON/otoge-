using Reilas;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shutter : MonoBehaviour
{
    [SerializeField] private GameObject jack;
    [SerializeField] private GameObject text;
    
    private Animator _anim;
    private static AllJudgeService _judgeService;
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
        _judgeService = gameObject.AddComponent<AllJudgeService>();

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
            case "ToR":
                SceneManager.LoadScene("ResultScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                AllNoteDestroy();
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
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                AllNoteDestroy();
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

        for (int a = RhythmGamePresenter.TapNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.TapNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.TapKujoNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.TapKujoNotes[a].NoteDestroy(true);
        }
        for (int a = RhythmGamePresenter.HoldNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.HoldNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.HoldKujoNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.HoldKujoNotes[a].NoteDestroy(true);
        }
        for (int a = RhythmGamePresenter.AboveTapNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveTapNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.AboveKujoTapNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveKujoTapNotes[a].NoteDestroy(true);
        }
        for (int a = RhythmGamePresenter.AboveSlideNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveSlideNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.AboveKujoSlideNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveKujoSlideNotes[a].NoteDestroy(true);
        }
        for (int a = RhythmGamePresenter.AboveHoldNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveHoldNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.AboveKujoHoldNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveKujoHoldNotes[a].NoteDestroy(true);
        }
        for (int a = RhythmGamePresenter.AboveChainNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveChainNotes[a].NoteDestroy(false);
        }
        for (int a = RhythmGamePresenter.AboveKujoChainNotes.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.AboveKujoChainNotes[a].NoteDestroy(true);
        }
        for(int a = RhythmGamePresenter.BarLines.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.BarLines[a].BarLineDestroy();
        }

        foreach (var lane in RhythmGamePresenter.TapNoteLanes) lane.Clear();
        RhythmGamePresenter.internalNotes.Clear();
        RhythmGamePresenter.chainNotes.Clear();
        RhythmGamePresenter.BarLines.Clear();
    }
}
