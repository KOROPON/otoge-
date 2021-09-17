using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MusicNumManage : MonoBehaviour
{
    private Image _jack;
    private Image _rank;
    private AudioSource _audioSource;
    private GetHighScores _getHighScores;
    private LevelConverter _levelConverter;
    private string _songName;
    private string _jacketPath;


    public Text highScore;
    public Text title;
    public Text easyLevel;
    public Text hardLevel;
    public Text extremeLevel;
    //public Text kujoLevel;
    public GameObject scrollviewContent;



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

    private void DisplayRank(string songName, string diff)
    {
        string rank = _getHighScores.GetRank(songName, diff);
        if (rank != "")
        {
            _rank.sprite = Resources.Load<Sprite>("Rank/score_" + rank);
        }
        else
        {
            _rank.sprite = null;
        }
    }

    private void DisplayLevel(string songName)
    {
        easyLevel.text = _levelConverter.GetLevel(songName, "Easy").ToString();
        hardLevel.text = _levelConverter.GetLevel(songName, "Hard").ToString();
        extremeLevel.text = _levelConverter.GetLevel(songName, "Extreme").ToString();
        //kujoLevel.text = _levelConverter.GetLevel(songName, "KUJO").ToString();
    }

    private void SelectSong(string musicName)
    {
        _jacketPath = "Jacket/" + musicName + "_jacket";
        MusicInfo("Songs/Music Select/" + musicName + "_intro", _jacketPath);
        title.text = musicName;
        PlayerPrefs.SetString("selected_song", musicName);
        _songName = musicName;
        string diff = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
        DisplayRank(_songName, diff);
        DisplayLevel(_songName);
    }

    void Start()
    {
        _jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        _rank = GameObject.Find("ランク").GetComponent<Image>();
        _audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        scrollviewContent = GameObject.Find("Content");
        _getHighScores = FindObjectOfType<GetHighScores>();
        _levelConverter = FindObjectOfType<LevelConverter>();
        

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

    public void Difficulty(GameObject diff)
    {
        PlayerPrefs.SetString("difficulty", diff.name);
        RhythmGamePresenter.dif = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff.name),9: 0,000,000}";
        DisplayRank(_songName, diff.name);


        for (int i = 0; i < scrollviewContent.transform.childCount; i++)
        {
            GameObject song = scrollviewContent.transform.GetChild(i).gameObject;
        }
    }
}
