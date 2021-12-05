using System;
using System.Collections;
using ShutterScene;
using UnityEngine;
using UnityEngine.UI;

namespace Select_Scene
{
    public class MusicNumManage : MonoBehaviour
    {
        private AudioSource _audioSource;
        private GameObject _kujo;
        private GameObject _extreme;
        private GetHighScores _getHighScores;
        private Image _jack;
        private Image _rank;
        private Image _frame;
        private Image _tutorialImage;
        private Text _tutorialText;
        private Text _composer;
        private Transform _scrollView;
        private Transform _scrollViewContent;
        
        private string _songName;
        private string _jacketPath;

        private bool _isExtreme;
        private bool _selectBool;
        private bool _blChange;
        private bool _blDifChange;
        
        private int _tutorialNum;

        public AudioSource audioO;
        public GameObject tutorial;
        public Text easyLevel;
        public Text hardLevel;
        public Text extremeLevel;
        public Text highScore;
        public Text title;

        private void MusicInfo(string musicName,string jacketPath)
        {
            _jacketPath = jacketPath;
            _audioSource.clip = Resources.Load<AudioClip>(musicName);
            _jack.sprite = Resources.Load<Sprite>(_jacketPath);
        }

        private void DisplayRank(string songName, string diff)
        {
            var rank = _getHighScores.GetRank(songName, diff);
            
            if (rank != "") _rank.sprite = Resources.Load<Sprite>("Rank/rank_" + rank);
            else
            {
                _rank.color = new Color32(255, 255, 255, 0);
                _rank.sprite = null;
            }
        }

        private void ChangeLevel(string songName)
        {
            easyLevel.text = LevelConverter.GetLevel(songName, "Easy").ToString();
            hardLevel.text = LevelConverter.GetLevel(songName, "Hard").ToString();
            extremeLevel.text = LevelConverter.GetLevel(songName, "Extreme").ToString();
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
            _kujo.SetActive(_getHighScores.GetKujoLock(musicName));
            
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
            
            if (!_blChange) StartCoroutine(JumpToSong(_songName));
            
            var diff = PlayerPrefs.GetString("difficulty");
            
            highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
            
            DisplayRank(_songName, diff);
            ChangeLevel(musicName);
            _audioSource.Play();
        }

        private void Start()
        {
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
            easyLevel = GameObject.Find("Easy").GetComponentInChildren<Text>();
            hardLevel = GameObject.Find("Hard").GetComponentInChildren<Text>();
            extremeLevel = GameObject.Find("Extreme").GetComponentInChildren<Text>();

            if (!PlayerPrefs.HasKey("selected_song")) PlayerPrefs.SetString("selected_song", "Collide");

            if (!PlayerPrefs.HasKey("difficulty")) PlayerPrefs.SetString("difficulty", "Easy");
            
            SelectSong(PlayerPrefs.GetString("selected_song"));
            
            if (PlayerPrefs.GetString("difficulty") == "Kujo")
            {
                _isExtreme = true;
                
                Difficulty("Extreme");
            }
            else Difficulty(PlayerPrefs.GetString("difficulty"));
            
            Shutter.blShutterChange = "Open"; 
            
            _audioSource.Play();

            if (PlayerPrefs.HasKey("PPtutorial")) return;
            // "Init"のキーが存在しない場合はチュートリアルパネルを表示
            // ”Init”のキーをint型の値(1)で保存
            PlayerPrefs.SetInt("PPtutorial", 1);
            PlayerPrefs.Save();
            tutorial.gameObject.SetActive(true);
            
            _tutorialImage = GameObject.Find("TutorialImage").GetComponent<Image>();
            _tutorialText = GameObject.Find("TutorialText").GetComponent<Text>();
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
            else SelectSong(obj.name);
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
                case "Extreme" when _isExtreme:
                {
                    blKujo = true;
                    
                    StartCoroutine("ExchangeDifficultyToKujo");
                    
                    diff = "Kujo";
                    
                    break;
                }
                case "Kujo":
                {
                    StartCoroutine("ExchangeDifficultyToExtreme");
                    
                    diff = "Extreme";
                    
                    break;
                }
                case "Extreme" when _kujo.activeSelf:
                {
                    _isExtreme = true;
                    
                    break;
                }
                default:
                {
                    _isExtreme = false;
                    
                    break;
                }
            }

            GetComponent<SongButtonSpawner>().SpawnSongs(blKujo);
            PlayerPrefs.SetString("difficulty", diff);
            
            RhythmGamePresenter.dif = PlayerPrefs.GetString("difficulty");
            highScore.text = $"{_getHighScores.GetHighScore(_songName, diff),9: 0,000,000}";
            
            DisplayRank(_songName, diff);
            
            //new Color32(color.color.r, color.color.g, color.color.b, color.color.a);
            _frame.color = GetColor(diff);

            for (var i = 0; i < _scrollViewContent.childCount; i++)
            {
                var song = _scrollViewContent.GetChild(i).gameObject;
                
                song.GetComponent<Image>().sprite = Resources.Load<Sprite>("Frame/" + diff);
                
                var rankSprite = song.GetComponentsInChildren<Image>()[1];
                var songLock = song.GetComponentsInChildren<Image>()[2];
                var determineButton = song.GetComponent<Button>();
                var clearGauge = song.GetComponentsInChildren<Image>()[3];
                var allowedLevel = song.GetComponentsInChildren<Text>()[1];

                clearGauge.sprite = _getHighScores.GetClear(song.name, diff) != null
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
                
                if (rank != "") rankSprite.sprite = Resources.Load<Sprite>("Rank/rank_" + rank);
                else rankSprite.color = Color.clear;
                
                foreach (var t in song.GetComponentsInChildren<Text>())
                    if (t.name == "Level")
                        t.text = LevelConverter.GetLevel(song.name, diff).ToString();
                
                switch (diff)
                {
                    case "Extreme" when song.name == "Reilas" && PlayerPrefs.HasKey("解禁状況") && !_getHighScores.GetKujoLock("Reilas"):
                        allowedLevel.text = PlayerPrefs.GetFloat("解禁状況") + " %";
                        
                        break;
                    case "Kujo":
                        GameObject.Find("Reilas").GetComponentsInChildren<Text>()[2].text = "10";
                        
                        break;
                }
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
        }

        public IEnumerator ExchangeDifficultyToExtreme()
        {
            Debug.Log("aaaa");
            
            var trueDif = _extreme;
            var falseDif = _kujo;
            var trueDifT = new Vector2(1670, 105);
            var falseDifT = new Vector2(1640, 75);
            
            trueDif.transform.SetAsFirstSibling();
            
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (Math.Abs(trueDifT.x - 1655) < 0) trueDif.transform.SetAsLastSibling();
                
                if (trueDifT.x > 1640)
                {
                    trueDifT = new Vector2(trueDifT.x - 1, trueDifT.y - 1);
                    falseDifT = new Vector2(falseDifT.x + 1, falseDifT.y + 1);
                    
                    continue;
                }
                
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
                    trueDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        trueDif.transform.GetComponent<RectTransform>().anchoredPosition.x - 1,
                        trueDif.transform.GetComponent<RectTransform>().anchoredPosition.y - 1);
                    falseDif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        falseDif.transform.GetComponent<RectTransform>().anchoredPosition.x + 1,
                        falseDif.transform.GetComponent<RectTransform>().anchoredPosition.y + 1);
                    
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
            switch (_tutorialNum)
            {
                case 0:
                    _tutorialText.text = "ここではあなたが挑戦することになる\n5種類の Note を紹介します。";
                    
                    _tutorialNum++;
                    
                    break;
                case 1:
                    _tutorialText.text = "まずはこの Tap-Note 。\nこのノーツはその名の通り押してとる Note です。";
                    
                    _tutorialNum++;
                    
                    break;
                case 2:
                {
                    var image = Resources.Load<Sprite>("Tutorial/Hold");
                    
                    _tutorialImage.sprite = image;
                    _tutorialText.text = "次に Hold-Note です。\nノーツの始まりから終わりまで押し続ける必要があります。";
                    
                    _tutorialNum++;
                    
                    break;
                }
                case 3:
                {
                    var image = Resources.Load<Sprite>("Tutorial/AboveTap");
                    
                    _tutorialImage.sprite = image;
                    _tutorialText.text = "これは AboveTap-Note です。\nTap-Note が上に移動したものです。";
                    
                    _tutorialNum++;
                    
                    break;
                }
                case 4:
                {
                    var image = Resources.Load<Sprite>("Tutorial/AboveSlide");
                    
                    _tutorialImage.sprite = image;
                    _tutorialText.text = "これは AboveSlide-Note です。\n押し方は Hold-Note と同じですが、上のレーンを上下左右に動きます。";
                    
                    _tutorialNum++;
                    
                    break;
                }
                case 5:
                {
                    var image = Resources.Load<Sprite>("Tutorial/Chain");
                    
                    _tutorialImage.sprite = image;
                    _tutorialText.text =
                        "これは Chain-Note です。\nこのノーツは Tap-Note と同じように見えますが、\nTap-Note と違って押し続けるだけで全ての判定をとることができます。";
                    
                    _tutorialNum++;
                    
                    break;
                }
                case 6:
                    _tutorialText.text = "これで紹介は終わりです。\n健闘を祈っています......";
                    
                    _tutorialNum++;
                    
                    break;
                default:
                    tutorial.gameObject.SetActive(false);
                    
                    break;
            }
        }
    }
}