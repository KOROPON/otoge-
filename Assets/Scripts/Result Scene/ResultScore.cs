using System;
using System.Diagnostics;
using Reilas;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    private AudioSource _resultMusic;
    private Image _jackInResult;
    private Image _rankInResult;
    private Text _scoreInResult;
    private Text _previousScoreText;
    private Text _scoreGap;
    private Text _maxCombo;
    private Text _perfectCom;
    private Text _goodCom;
    private Text _badCom;
    private Text _missCom;
    private Text _titleInResult;
    private Text _difficultyInResult;
    private Text _rankDifficulty;
    private Image _resultColor;
    private Image _clearRank;

    private GetHighScores _getHighScores;
    private LevelConverter _levelConverter;
    private bool _backBool;
    private bool _retryBool;
    private int _previousScore;
    private int _score;
    private string _scoreRank;


    private void Start()
    {
        _getHighScores = gameObject.AddComponent<GetHighScores>();
        _score = ChangeScenePlayScene.score;
        _previousScore = ChangeScenePlayScene.previousHighScore;
        _scoreRank = _getHighScores.GetRank(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
        _backBool = true;
        _retryBool = true;
        _scoreInResult = GameObject.Find("Score").GetComponent<Text>();
        _maxCombo = GameObject.Find("Combo").GetComponent<Text>();
        _perfectCom = GameObject.Find("PerfectCom").GetComponent<Text>();
        _goodCom = GameObject.Find("GoodCom").GetComponent<Text>();
        _badCom = GameObject.Find("BadCom").GetComponent<Text>();
        _missCom = GameObject.Find("MissCom").GetComponent<Text>();
        _titleInResult = GameObject.Find("Name").GetComponent<Text>();
        _difficultyInResult = GameObject.Find("Difficulty").GetComponent<Text>();
        _rankDifficulty = GameObject.Find("DifficultyRank").GetComponent<Text>();
        _jackInResult = GameObject.Find("Jacket").GetComponent<Image>();
        _rankInResult = GameObject.Find("Rank").GetComponent<Image>();
        _previousScoreText = GameObject.Find("PreviousScore").GetComponent<Text>();
        _scoreGap = GameObject.Find("ScoreGap").GetComponent<Text>();
        _resultMusic = GameObject.Find("Theme").GetComponent<AudioSource>();
        _resultColor = GameObject.Find("JacketFrame").GetComponent<Image>();
        _clearRank = GameObject.Find("Clear").GetComponent<Image>();
        _levelConverter = gameObject.AddComponent<LevelConverter>();

        _scoreInResult.text = $"{_score:0,000,000}";
        _maxCombo.text = ScoreComboCalculator.highCombo.ToString();
        _perfectCom.text = ScoreComboCalculator.sumPerfect.ToString();
        _goodCom.text = ScoreComboCalculator.sumGood.ToString();
        _badCom.text = ScoreComboCalculator.sumBad.ToString();
        _missCom.text = ScoreComboCalculator.sumMiss.ToString();
        _titleInResult.text = RhythmGamePresenter.musicName;
        _difficultyInResult.text = RhythmGamePresenter.dif;
        _jackInResult.sprite = Resources.Load<Sprite>("Jacket/" + _titleInResult.text + "_jacket");
        _rankInResult.sprite = Resources.Load<Sprite>("Rank/rank_" + _scoreRank);
        _previousScoreText.text = $"{_previousScore:0,000,000}";
        _scoreGap.text = _score switch
        {
            var n when n > _previousScore => "+" + $"{n - _previousScore:0,000,000}",
            var n when n < _previousScore => "-" + $"{_previousScore - n:0,000,000}",
            _=> ""
        };

        _clearRank.sprite = Resources.Load<Sprite>("ClearRank/" + ChangeScenePlayScene.clear);
        _rankDifficulty.text = LevelConverter.GetLevel(RhythmGamePresenter.musicName, RhythmGamePresenter.dif).ToString();
        _resultColor.color = RhythmGamePresenter.dif switch
        {
            "Easy" => new Color32(9, 135, 128, 255),
            "Hard" => new Color32(135, 133, 9, 255),
            "Extreme" => new Color32(120, 9, 135, 255),
            "Kujo" => new Color32(150, 150, 150, 255),
        };
        //未実装00

        Shutter.blChange = "Open";
        Shutter.blShutterChange = "Open";
        _resultMusic.Play();
    }

    public void Back()
    {
        if (!_backBool) return;
        _backBool = false;
        _resultMusic.Stop();
        //シャッター下げる
        Shutter.blChange = "ToSFrR";
        Shutter.blShutterChange = "Close";
        Invoke(nameof(ResultStop),0.5f);
    }

    public void Retry()
    {
        if (!_retryBool) return;
        _retryBool = false;
        //シャッター下げる
        Shutter.blChange = "ToPFrR";
        Shutter.blShutterChange = "Close";
        Invoke(nameof(ResultStop),0.5f);
    }
    private void ResultStop()
    {
      _resultMusic.Stop();
    }
}
