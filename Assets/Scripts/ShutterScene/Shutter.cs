using Reilas;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shutter : MonoBehaviour
{
    [SerializeField] private GameObject jack;
    [SerializeField] private GameObject name;
    [SerializeField] private GameObject com;
    [SerializeField] private GameObject des;
    [SerializeField] private GameObject dif;
    [SerializeField] private GameObject pan;

    private Animator _anim;
    private static AllJudgeService _judgeService;
    private Image _jacket;
    private Text _title;
    private Text _composer;
    private Text _noteDesigner;
    private Text _difficulty;
    private Image _panel;
    private RhythmGamePresenter _presenter;

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
        name.SetActive(true);
       com.SetActive(true);
        des.SetActive(true);
        dif.SetActive(true);
        pan.SetActive(true);
        _jacket = jack.GetComponent<Image>();
        _title = name.GetComponent<Text>();
        _composer = com.GetComponent<Text>();
        _noteDesigner = des.GetComponent<Text>();
        _difficulty = dif.GetComponent<Text>();
        _panel = pan.GetComponent<Image>();
        jack.SetActive(false);
        name.SetActive(false);
        des.SetActive(false);
        com.SetActive(false);
        dif.SetActive(false);
        pan.SetActive(true);
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
                name.SetActive(true);
                com.SetActive(true);
                des.SetActive(true);
                dif.SetActive(true);
                pan.SetActive(true);
                var songName = RhythmGamePresenter.musicName;
                var difficulty = RhythmGamePresenter.dif;
                _jacket.sprite = Resources.Load<Sprite>("Jacket/" + songName + "_jacket");
                _title.text = songName;
                _composer.text = "Composer:" + LevelConverter.GetComposer(songName);
                _noteDesigner.text = "NoteDesigner:" + LevelConverter.GetNoteEditor(songName, difficulty);
                _difficulty.text = LevelConverter.GetLevel(songName, difficulty).ToString();
                _panel.sprite = Resources.Load<Sprite>("Frame/Square");
                switch (difficulty)
                {
                    case "Easy": _difficulty.color = new Color32(0, 255, 50, 255);break;
                    case "Hard": _difficulty.color = new Color32(255, 210, 0, 255);break;
                    case "Extreme": _difficulty.color = new Color32(100, 0, 255, 255);break;
                    case "Kujo": _difficulty.color = new Color32(140, 180, 175, 255); break;
                }
                if (_title.text == "")
                {
                    _title.text = "Tutorial";
                    _jacket.sprite = Resources.Load<Sprite>("Jacket/IconMain");
                    _panel.sprite = Resources.Load<Sprite>("Jacket/IconMain");
                }
                _anim.SetBool("blToPlay", true);
                break;
        }
    }

    private void CloseFunction()
    {
        switch (blChange)
        {
            case "ToPFrP":
                _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
                AllNoteDestroy();
                _presenter._reilasAboveHold.Clear();
                _presenter._reilasAboveSlide.Clear();
                _presenter._reilasHold.Clear();
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
                SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
                break;
            case "ToR":
                _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
                AllNoteDestroy();
                _presenter._reilasAboveHold.Clear();
                _presenter._reilasAboveSlide.Clear();
                _presenter._reilasHold.Clear();
                SceneManager.LoadScene("ResultScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
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
                _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
                AllNoteDestroy();
                _presenter._reilasAboveHold.Clear();
                _presenter._reilasAboveSlide.Clear();
                _presenter._reilasHold.Clear();
                SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
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

        for(int a = RhythmGamePresenter.NoteConnectors.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.NoteConnectors[a].NoteConnectorDestroy();
        }
        for(int a = RhythmGamePresenter.NoteKujoConnectors.Count() - 1; a >= 0; a--)
        {
            RhythmGamePresenter.NoteKujoConnectors[a].NoteConnectorDestroy();
        }

        foreach (var lane in RhythmGamePresenter.TapNoteLanes) lane.Clear();

        RhythmGamePresenter.internalNotes.Clear();
        RhythmGamePresenter.chainNotes.Clear();
        RhythmGamePresenter.BarLines.Clear();
    }
}
