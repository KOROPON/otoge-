using System;
using System.Diagnostics;
using Reilas;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    public Text scoreInResult;
    public Text previousScore;
    public Text scoreGap;
    public Text maxCombo;
    public Text perfectCom;
    public Text goodCom;
    public Text badCom;
    public Text missCom;
    public Text titleInResult;
    public Text difficultyInResult;
    public Text rankDifficulty;
    public Image jackinResult;
    public Image rankinResult;
    public Image colorinResult;
    public Image clear;
    public AudioSource resultMusic;

    private GetHighScores _getHighScores;
    private ChangeScene_PlayScene _previousHighScore;
    private int _previousScore;
    private int _score;
    private string _scoreRank;
    private bool _backBool;
    private bool _retryBool;


    private void Start()
    {
        _getHighScores = gameObject.AddComponent<GetHighScores>();
        _previousHighScore = gameObject.AddComponent<ChangeScene_PlayScene>();

        _previousScore = _previousHighScore.previousHighScore;
        _score = _getHighScores.GetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif);
        _backBool = true;
        _retryBool = true;
        
        scoreInResult.text = $"{_score}";
        maxCombo.text = ScoreComboCalculator.highCombo.ToString();
        perfectCom.text = ScoreComboCalculator.sumPerfect.ToString();
        goodCom.text = ScoreComboCalculator.sumGood.ToString();
        badCom.text = ScoreComboCalculator.sumBad.ToString();
        missCom.text = ScoreComboCalculator.sumMiss.ToString();
        titleInResult.text = RhythmGamePresenter.musicname;
        difficultyInResult.text = RhythmGamePresenter.dif;
        jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleInResult.text + "_jacket");
        rankinResult.sprite = Resources.Load<Sprite>("Rank/rank_" + _scoreRank);
        previousScore.text = $"{_previousScore}";
        scoreGap.text = _score switch
        {
            int n when n > _previousScore => "+" + $"{n - _previousScore}",
            int n when n < _previousScore => "-" + $"{_previousScore - n}",
            _=> ""
        };
        
        //clear.sprite =Resources.Load<Sprite>()
        //rankDifficulty =
        //colorinResult =
        //未実装00

        Shutter.blChange = "Open";
        Shutter.blShutterChange = "Open";
        resultMusic.Play();
    }

    public void Back()
    {
        if (!_backBool) return;
        _backBool = false;
        resultMusic.Stop();
        Shutter.blChange = "ToSFrR";//シャッター下げる
        Shutter.blShutterChange = "Close";
        Invoke("ResultStop",0.5f);
    }

    public void Retry()
    {
        if (!_retryBool) return;
        _retryBool = false;
        Shutter.blChange = "ToPFrR";//シャッター下げる
        Shutter.blShutterChange = "Close";
        Invoke("ResultStop",0.5f);
    }
    private void ResultStop()
    {
      resultMusic.Stop();
    }
}
