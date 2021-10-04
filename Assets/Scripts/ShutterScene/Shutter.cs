using Reilas;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shutter : MonoBehaviour
{
    private Animator anim;
    private Text _title;
    [SerializeField]private GameObject _jack;
    [SerializeField] private GameObject _text;
    private Image _jacket;

    public static string blChange;
    public static string blShutterChange;
    public static bool bltoPlay = false;
    public AudioSource openSE;
    public AudioSource closeSE;
    public static AudioSource music_m;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        _jack.SetActive(true);
        _text.SetActive(true);
        _jacket = _jack.GetComponent<Image>();
        _title = _text.GetComponent<Text>();
        _jack.SetActive(false);
        _text.SetActive(false);
        music_m = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        switch (blShutterChange)
        {
            case "Open":
                anim.SetBool("bl", false);
                anim.SetBool("blToPlay", false);
                break;
            case "Close":
                anim.SetBool("bl", true);
                break;
            case "CloseToPlay":
                _jack.SetActive(true);
                _text.SetActive(true);
                _jacket.sprite = Resources.Load<Sprite>("Jacket/" + RhythmGamePresenter.musicname + "_jacket");
                _title.text = RhythmGamePresenter.musicname;
                anim.SetBool("blToPlay", true);
                break;
        }
    }

    void CloseFunction()
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

    void CloseToPlayFunction()
    {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("SelectScene", UnloadSceneOptions.None);
    }
   
    void PlaySongAudio()
    {
        if (bltoPlay)
        {
            Invoke("PlayAudio", 1f);
            bltoPlay = false;
        }
    }
   
    void OpenAudio()
    {
        openSE.Play();
    }
    
    void CloseAudio()
    {
        closeSE.Play();
    }
   
    
    
    void PlayAudio()
    {
        RhythmGamePresenter.PlaySongs();
        ChangeScene_PlayScene.playNoticed = true;
        SettingField.SetBool = true;
    }
    
    private void AllNoteDestroy()
    {

        foreach (var a in RhythmGamePresenter._tapNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter._holdNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter._aboveTapNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter._aboveSlideNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter._aboveHoldNotes)
        {
            Destroy(a.gameObject);
        }
        foreach (var a in RhythmGamePresenter._aboveChainNotes)
        {
            Destroy(a.gameObject);
        }
        RhythmGamePresenter.tapNotes.Clear();
        RhythmGamePresenter.internalNotes.Clear();
        RhythmGamePresenter.chainNotes.Clear();
        RhythmGamePresenter._tapNotes.Clear();
        RhythmGamePresenter._holdNotes.Clear();
        RhythmGamePresenter._aboveTapNotes.Clear();
        RhythmGamePresenter._aboveSlideNotes.Clear();
        RhythmGamePresenter._aboveHoldNotes.Clear();
        RhythmGamePresenter._aboveChainNotes.Clear();
        RhythmGamePresenter._barLines.Clear();
        BarLine.BarLines.Clear();
    }
}
