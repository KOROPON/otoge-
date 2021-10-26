using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MusicNumManage : MonoBehaviour
{
    private Image _jack;
    private Image _rank;
    private Image _frame;
    private AudioSource _audioSource;
    private GetHighScores _getHighScores;
    private Transform _scrollView;
    private Transform _scrollviewContent;
    private LevelConverter _levelConverter;
    private string _songName;
    private string _jacketPath;
    private bool _selectBool;
    private bool _blChange;
    private bool _blDifChange;

    public Text highScore;
    public Text title;
    public Text easyLevel;
    public Text hardLevel;
    public Text extremeLevel;
    public AudioSource audioO;
    
    //public Text kujoLevel;



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
            _rank.sprite = Resources.Load<Sprite>("Rank/rank_" + rank);
        }
        else
        {
            _rank.sprite = null;
        }
    }

    private void ChangeLevel(string songName)
    {
        easyLevel.text = _levelConverter.GetLevel(songName, "Easy").ToString();
        hardLevel.text = _levelConverter.GetLevel(songName, "Hard").ToString();
        extremeLevel.text = _levelConverter.GetLevel(songName, "Extreme").ToString();
    }

    private IEnumerator JumpToSong(string songName)
    {
        for (int i = 0; i < _scrollviewContent.childCount; i++)
        {
            Transform childSongTrans = _scrollviewContent.GetChild(i);
            if (childSongTrans.gameObject.name == songName)
            {
                _blChange = true;
                Vector3 localPosition = _scrollviewContent.localPosition;
                Vector3 goal = new Vector3(localPosition.x, -childSongTrans.localPosition.y,
                    localPosition.z);
                while (Vector3.Distance(_scrollviewContent.localPosition, goal) > 1)
                {
                    _scrollviewContent.localPosition = Vector3.Lerp(_scrollviewContent.localPosition,
                       goal, 0.4f);
                    yield return null;
                }
                _blChange = false;
            }
        }
    }
    
    private void SelectSong(string musicName)
    {
        _jacketPath = "Jacket/" + musicName + "_jacket";
        MusicInfo("Songs/Music Select/" + musicName + "_intro", _jacketPath);
        title.text = musicName;
        if (!_getHighScores.GetLock(musicName))
        {
            GameObject extreme = GameObject.Find("Extreme");
            extreme.GetComponent<Image>().color = new Color32(174, 174, 174, 255);
            extreme.GetComponent<Button>().enabled = false;
            extreme.GetComponentsInChildren<Image>()[1].enabled = true;
        }
        else
        {
            GameObject extreme = GameObject.Find("Extreme");
            extreme.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            extreme.GetComponent<Button>().enabled = true;
            extreme.GetComponentsInChildren<Image>()[1].enabled = false;
        }
        PlayerPrefs.SetString("selected_song", musicName);
        _songName = musicName;
        if (!_blChange)
        {
            StartCoroutine(JumpToSong(_songName));
        }
        string diff = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
        DisplayRank(_songName, diff);
        ChangeLevel(musicName);
        _audioSource.Play();
    }

    void Start()
    {
        _selectBool = true;
        _jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        _rank = GameObject.Find("ランク").GetComponent<Image>();
        _frame = GameObject.Find("Frame").GetComponent<Image>();
        _audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        _scrollView = GameObject.Find("Scroll View").transform;
        _scrollviewContent = _scrollView.GetChild(0);
        _getHighScores = FindObjectOfType<GetHighScores>();
        _levelConverter = FindObjectOfType<LevelConverter>();
        
        FindObjectOfType<SongButtonSpawner>().SpawnSongs();

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
        Shutter.blShutterChange = "Open"; 
        _audioSource.Play();
    }

    public void Tap(GameObject obj)
    {
        if (PlayerPrefs.GetString("selected_song") == obj.name)
        {
            if (_selectBool)
            {
                RhythmGamePresenter.musicName = obj.name;
                _selectBool = false;
                Shutter.blShutterChange = "CloseToPlay";
                _audioSource.Stop();
                audioO.Play();
            }
        }
        else
        {
            SelectSong(obj.name);
        }
    }

    public Color32 GetColor(string diff)
    {
        return diff switch
        {
            "Easy" => new Color32(9, 135, 128, 255),
            "Hard" => new Color32(135, 133, 9, 255),
            "Extreme" => new Color32(128, 9, 135, 255),
            _ => new Color32()
        };
    }

   
    public void Difficulty(GameObject diff)
    {
        PlayerPrefs.SetString("difficulty", diff.name);
        RhythmGamePresenter.dif = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff.name),9: 0,000,000}";
        DisplayRank(_songName, diff.name);
        _frame.color = GetColor(diff.name);//new Color32(color.color.r, color.color.g, color.color.b, color.color.a);

        for (int i = 0; i < _scrollviewContent.childCount; i++)
        {
            GameObject song = _scrollviewContent.GetChild(i).gameObject;
            song.GetComponent<Image>().sprite = Resources.Load<Sprite>("Frame/" + diff.name);
            Image rankSprite = song.GetComponentsInChildren<Image>()[1];
            Image songLock = song.GetComponentsInChildren<Image>()[2];
            Button determineButton = song.GetComponent<Button>();
            if (diff.name == "Extreme" && !_getHighScores.GetLock(song.name))
            {
                songLock.enabled = true;
                determineButton.enabled = false;
                song.GetComponent<Image>().color = new Color32(174, 174, 174, 255);
            }
            else
            {
                songLock.enabled = false;
                determineButton.enabled = true;
                song.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }

            string rank = _getHighScores.GetRank(song.name, diff.name);
            if (rank != "")
            {
                rankSprite.sprite = Resources.Load<Sprite>("Rank/score_" + rank);
            }
            else
            {
                rankSprite.color = Color.clear;
            }
            foreach (Text t in song.GetComponentsInChildren<Text>())
            {
                if (t.name == "Level")
                {
                    t.text = _levelConverter.GetLevel(song.name, diff.name).ToString();
                }
            }
        }
    }
    private IEnumerator JumpInDifficulty()
    {
        _blDifChange = true;
        Vector3 goal = _scrollView.localPosition;
        _scrollView.localPosition = new Vector3(750, goal.y, goal.z);
        while (Vector3.Distance(_scrollView.localPosition, goal) > 1)
        {
            _scrollView.localPosition = Vector3.Lerp(_scrollView.localPosition,
               goal, 0.4f);
            yield return null;
        }
        _blDifChange = false;
    }

    public void DifficultAnim()
    {
        if (!_blDifChange)
        {
            StartCoroutine(JumpInDifficulty());
            Debug.Log("s");
        }
    }
    
}
