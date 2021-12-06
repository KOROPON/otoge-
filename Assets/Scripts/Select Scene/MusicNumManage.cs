using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShutterScene;

public class MusicNumManage : MonoBehaviour
{
    private Image _jack;
    private Image _rank;
    private Image _frame;
    private Image _tutorialImage;
    private AudioSource _audioSource;
    private GetHighScores _getHighScores;
    public Text _easyLevel;
    public Text _hardLevel;
    public Text _extremeLevel;
    private Text _tutorialText;
    private GameObject _kujo;
    private GameObject _extreme;
    private GameObject _tutorial;
    private Transform _scrollView;
    private Transform _scrollViewContent;
    private string _songName;
    private string _jacketPath;
    private bool _selectBool;
    private bool _blChange;
    private bool _blDifChange;
    private int _tutorialNum = 0;

    public Text highScore;
    public Text title;
    private Text _composer;
    public AudioSource audioO;
    private bool isExtreme;

    //public Text kujoLevel;



    private GameObject GetDifficulty(string diff)
    {
        var getDiff = diff switch
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
        var rank = _getHighScores.GetRank(songName, diff);
        if (rank != "")
        {
            _rank.sprite = Resources.Load<Sprite>("Rank/rank_" + rank);
        }
        else
        {
            _rank.color = new Color32(255, 255, 255, 0);
            _rank.sprite = null;
        }
    }

    private void ChangeLevel(string songName)
    {
        _easyLevel.text = LevelConverter.GetLevel(songName, "Easy").ToString();
        _hardLevel.text = LevelConverter.GetLevel(songName, "Hard").ToString();
        _extremeLevel.text = LevelConverter.GetLevel(songName, "Extreme").ToString();
    }

    private IEnumerator JumpToSong(string songName)
    {
        for (var i = 0; i < _scrollViewContent.childCount; i++)
        {
            var childSongTrans = _scrollViewContent.GetChild(i);
            if (childSongTrans.gameObject.name != songName) continue;
            _blChange = true;
            var localPosition = _scrollViewContent.localPosition;
            var goal = new Vector3(localPosition.x, -childSongTrans.localPosition.y,
                localPosition.z);
            while (Vector3.Distance(_scrollViewContent.localPosition, goal) > 1)
            {
                _scrollViewContent.localPosition = Vector3.Lerp(_scrollViewContent.localPosition, goal, 0.4f);
                yield return null;
            }
            _blChange = false;
        }
    }
    
    private void SelectSong(string musicName)
    {

        if (_getHighScores.GetKujoLock(musicName)) _kujo.SetActive(true);
        else _kujo.SetActive(false);
        _jacketPath = "Jacket/" + musicName + "_jacket";
        MusicInfo("Songs/Music Select/" + musicName + "_intro", _jacketPath);
        title.text = musicName;
        _composer.text = LevelConverter.GetComposer(musicName);
        if (!_getHighScores.GetLock(musicName))
        {
            var extreme = GameObject.Find("Extreme");
            extreme.GetComponent<Image>().color = new Color32(174, 174, 174, 255);
            extreme.GetComponent<Button>().enabled = false;
            extreme.GetComponentsInChildren<Image>()[1].enabled = true;
        }
        else
        {
            var extreme = GameObject.Find("Extreme");
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
        var diff = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
        DisplayRank(_songName, diff);
        ChangeLevel(musicName);
        _audioSource.Play();
    }

    private void Start()
    {
        _tutorial = GameObject.Find("Tutorial");
        _selectBool = true;
        _kujo = GameObject.Find("Kujo");
        _extreme = GameObject.Find("Extreme");
        _kujo.SetActive(false);
        _jack = GameObject.Find("ジャケット1").GetComponent<Image>();
        _rank = GameObject.Find("ランク").GetComponent<Image>();
        _frame = GameObject.Find("Frame").GetComponent<Image>();
        _audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        _scrollView = GameObject.Find("Scroll View").transform;
        _scrollViewContent = _scrollView.GetChild(0);
        _getHighScores = FindObjectOfType<GetHighScores>();
        _composer = GameObject.Find("Composer").GetComponent<Text>();
        _easyLevel = GameObject.Find("Easy").GetComponentInChildren<Text>();
        _hardLevel = GameObject.Find("Hard").GetComponentInChildren<Text>();
        _extremeLevel = GameObject.Find("Extreme").GetComponentInChildren<Text>();
        Debug.Log(_getHighScores.GetKujoLock("Collide"));

        if (!PlayerPrefs.HasKey("selected_song"))
        {
            PlayerPrefs.SetString("selected_song", "Collide");
        }

        if (!PlayerPrefs.HasKey("difficulty"))
        {
            PlayerPrefs.SetString("difficulty", "Easy");
        }
        SelectSong(PlayerPrefs.GetString("selected_song"));
        if (PlayerPrefs.GetString("difficulty") == "Kujo")
        {
            isExtreme = true;
            Difficulty("Extreme");
        }
        else
        {
            Difficulty(PlayerPrefs.GetString("difficulty"));
        }
        Shutter.blShutterChange = "Open"; 
        _audioSource.Play(); 
        if (!PlayerPrefs.HasKey("tutorialDebug17"))
        {   
            //キーが存在しない場合はチュートリアルパネルを表示
            PlayerPrefs.SetFloat("rate", 3);
            PlayerPrefs.Save();
            _tutorial.gameObject.SetActive(true);
            _tutorialImage = GameObject.Find("TutorialImage").GetComponent<Image>();
            _tutorialText = GameObject.Find("TutorialText").GetComponent<Text>();
        }
        else
        {
            _tutorial.gameObject.SetActive(false);
        }
    }

    public void Tap(GameObject obj)
    {
        if (PlayerPrefs.GetString("selected_song") == obj.name)
        {
            if (!_selectBool) return;
            RhythmGamePresenter.musicName = obj.name;
            _selectBool = false;
            Shutter.blShutterChange = "CloseToPlay";
            _audioSource.Stop();
            audioO.Play();
        }
        else
        {
            SelectSong(obj.name);
        }
    }

    private static Color32 GetColor(string diff)
    {
        return diff switch
        {
            "Easy" => new Color32(9, 135, 128, 255),
            "Hard" => new Color32(135, 133, 9, 255),
            "Extreme" => new Color32(128, 9, 135, 255),
            _ => new Color32()
        };
    }
    public void Difficulty(string diff)
    {
        var blKujo = false;
        
        switch (diff)
        {
            case "Extreme" when isExtreme:
                Debug.Log("sang");
                blKujo = true;
                StartCoroutine("ExchangeDifficultyToKujo");
                diff = "Kujo";
                break;
            case "Kujo":
                Debug.Log("baaba");
                StartCoroutine("ExchangeDifficultyToExtreme");
                diff = "Extreme";
                break;
            case "Extreme" when _kujo.activeSelf:
                Debug.Log("daaadda");
                isExtreme = true ;
                break;
            default:
                Debug.Log("klklklk");
                isExtreme = false;
                break;
        }

        GetComponent<SongButtonSpawner>().SpawnSongs(blKujo, LevelConverter.GetGameObject(diff));
        PlayerPrefs.SetString("difficulty", diff);
        RhythmGamePresenter.dif = PlayerPrefs.GetString("difficulty");
        highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
        DisplayRank(_songName, diff);
        _frame.color = GetColor(diff);//new Color32(color.color.r, color.color.g, color.color.b, color.color.a);

        for (var i = 0; i < _scrollViewContent.childCount; i++)
        {
            var song = _scrollViewContent.GetChild(i).gameObject;
            song.GetComponent<Image>().sprite = Resources.Load<Sprite>("Frame/" + diff);
            var rankSprite = song.GetComponentsInChildren<Image>()[1];
            var songLock = song.GetComponentsInChildren<Image>()[2];
            var determineButton = song.GetComponent<Button>();
            var clearGuage = song.GetComponentsInChildren<Image>()[3];
            var allowedLevel = song.GetComponentsInChildren<Text>()[1];
            clearGuage.sprite = _getHighScores.GetClear(song.name, diff) != null
                ? Resources.Load<Sprite>("ClearGuage/ClearGuage_" + _getHighScores.GetClear(song.name, diff))
                : Resources.Load<Sprite>("ClearGuage/ClearGuage_Failed");
            if (diff == "Extreme" && !_getHighScores.GetLock(song.name))
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

            var rank = _getHighScores.GetRank(song.name, diff);
            if (rank != "")
            {
                rankSprite.sprite = Resources.Load<Sprite>("Rank/rank_" + rank);
            }
            else
            {
                rankSprite.color = Color.clear;
            }
            foreach (var t in song.GetComponentsInChildren<Text>())
            {
                if (t.name == "Level")
                {
                    t.text = LevelConverter.GetLevel(song.name, diff).ToString();
                }
            }
            if (diff == "Extreme" && song.name == "Reilas" && PlayerPrefs.HasKey("解禁状況") && !_getHighScores.GetKujoLock("Reilas"))
            {
                allowedLevel.text = PlayerPrefs.GetFloat("解禁状況") + " %";
            }
            if(diff == "Kujo") GameObject.Find("Reilas").GetComponentsInChildren<Text>()[2].text = "10";
        }
    }
    private IEnumerator JumpInDifficulty()
    {
        _blDifChange = true;
        var goal = _scrollView.localPosition;
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
        if (_blDifChange) return;
        StartCoroutine(JumpInDifficulty());
        Debug.Log("s");
    }

    public IEnumerator ExchangeDifficultyToExtreme()
    {
        Debug.Log("aaaa");
        int i = 0;
        Transform transT;
        Transform transF;
        var trueDif = _extreme;
        var falseDif = _kujo;
        var trueDifT = (transT = trueDif.transform).GetComponent<RectTransform>().anchoredPosition;
        var falseDifT = (transF = falseDif.transform).GetComponent<RectTransform>().anchoredPosition;
        trueDifT = new Vector2(1670, 105);
        falseDifT = new Vector2(1640, 75);
        trueDif.transform.SetAsFirstSibling();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (trueDifT.x == 1655)
            {
                trueDif.transform.SetAsLastSibling();
            }
            if (trueDifT.x > 1640)
            {
                trueDifT = new Vector2(trueDifT.x - 1, trueDifT.y - 1);
                falseDifT = new Vector2(falseDifT.x + 1, falseDifT.y + 1);
                continue;
            }
            trueDifT = new Vector3(1640, 75);
            falseDifT = new Vector3(1670, 105);
            trueDif.transform.GetComponent<Button>().enabled = true;
            falseDif.transform.GetComponent<Button>().enabled = false;
            break;
        }
    }
    public IEnumerator ExchangeDifficultyToKujo()
    {
        var trueDif = _kujo;
        var falseDif = _extreme;
        trueDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(1670, 105);
        falseDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(1640, 75);
        trueDif.transform.SetAsFirstSibling();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (Math.Abs(trueDif.transform.GetComponent<RectTransform>().anchoredPosition.x - 1655) < 0)
                trueDif.transform.SetAsLastSibling();
            
            if (trueDif.transform.GetComponent<RectTransform>().anchoredPosition.x > 1640)
            {
                trueDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(trueDif.transform.GetComponent<RectTransform>().anchoredPosition.x - 1, (float)(trueDif.transform.GetComponent<RectTransform>().anchoredPosition.y - 1));
                falseDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(falseDif.transform.GetComponent<RectTransform>().anchoredPosition.x + 1, (float)(falseDif.transform.GetComponent<RectTransform>().anchoredPosition.y + 1));
                continue;
            }
            trueDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(1640, 75);
            falseDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(1670, 105);
            trueDif.transform.GetComponent<Button>().enabled = true;
            falseDif.transform.GetComponent<Button>().enabled = false;
            break;
        }
    }

    public void TapTutorial()
    {
        if(_tutorialNum == 0)
        {
            _tutorialText.text = "ここではあなたが挑戦することになる\n5種類の Note を紹介します。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 1)
        {
            _tutorialText.text = "まずはこの Tap-Note 。\nこのノーツはその名の通り押してとる Note です。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 2)
        {
            Sprite image = Resources.Load<Sprite>("Tutorial/Hold");
            _tutorialImage.sprite = image;
            _tutorialText.text = "次に Hold-Note です。\nノーツの始まりから終わりまで押し続ける必要があります。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 3)
        {
            Sprite image = Resources.Load<Sprite>("Tutorial/AboveTap");
            _tutorialImage.sprite = image;
            _tutorialText.text = "これは AboveTap-Note です。\nTap-Note が上に移動したものです。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 4)
        {
            Sprite image = Resources.Load<Sprite>("Tutorial/AboveSlide");
            _tutorialImage.sprite = image;
            _tutorialText.text = "これは AboveSlide-Note です。\n押し方は Hold-Note と同じですが、上のレーンを左右に動きます。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 5)
        {
            Sprite image = Resources.Load<Sprite>("Tutorial/Chain");
            _tutorialImage.sprite = image;
            _tutorialText.text = "これは Chain-Note です。\nこのノーツは Tap-Note と同じように見えますが、\nTap-Note と違って押し続けるだけで全ての判定をとることができます。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 6)
        {
            _tutorialText.text = "左上にあるゲージは、クリアの判定に使われます。\n終了時に 70% 以上でないと Failed となるので気をつけてください。";
            _tutorialNum++;
        }
        else if(_tutorialNum == 7)
        {
            _tutorialText.text = "では実際にプレイしてみましょう！";
            _tutorialNum++;
        }
        else
        {
            // tutorial をプレイ
            RhythmGamePresenter.tutorial = true;
            Shutter.blShutterChange = "CloseToPlay";
        }
    }
}