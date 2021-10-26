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
    private Text previousScore;
    private Text _scoreGap;
    private Text _maxCombo;
    private Text _perfectCom;
    private Text _goodCom;
    private Text _badCom;
    private Text _missCom;
    private Text _titleInResult;
    private Text _difficultyInResult;

    private GetHighScores _getHighScores;
    private bool _backBool;
    private bool _retryBool;
    private int _previousScore;
    private int _score;
    private string _scoreRank;


    private void Start()
    {
        _getHighScores = gameObject.AddComponent<GetHighScores>();
        _score = _getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
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
        _jackInResult = GameObject.Find("Jacket").GetComponent<Image>();
        _rankInResult = GameObject.Find("Rank").GetComponent<Image>();
        previousScore = GameObject.Find("Jacket").GetComponent<Text>();
        _scoreGap = GameObject.Find("ScoreGap").GetComponent<Text>();
        _resultMusic = GameObject.Find("Theme").GetComponent<AudioSource>();

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
        previousScore.text = $"{_previousScore:0,000,000}";
        _scoreGap.text = _score switch
        {
            var n when n > _previousScore => "+" + $"{n - _previousScore:0,000,000}",
            var n when n < _previousScore => "-" + $"{_previousScore - n:0,000,000}",
            _=> ""
        };
        
        //clear.sprite =Resources.Load<Sprite>()
        //rankDifficulty =
        //colorInResult =
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
