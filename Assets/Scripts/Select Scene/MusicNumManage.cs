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
    private bool selectBool;


    public Text highScore;
    public Text title;
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
        _levelConverter.GetLevel(musicName);
        _audioSource.Play();
    }

    void Start()
    {
        selectBool = true;
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
        //シャッター上げる to三ツ口 Startで必要な動作はここまでに抑えといてね
        _audioSource.Play();
    }

    public void Tap(GameObject obj)
    {
        if (PlayerPrefs.GetString("selected_song") == obj.name)
        {
            if (selectBool)
            {
              selectBool = false;
              _audioSource.Stop();
              //シャッター閉じる;
              RhythmGamePresenter.musicname = obj.name;
              SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
              SceneManager.UnloadSceneAsync("SelectScene", UnloadSceneOptions.None);
            }
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
