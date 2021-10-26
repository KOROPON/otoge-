using System;
using System.Diagnostics;
using Reilas;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    public AudioSource resultMusic;
    public Image jackInResult;
    public Image rankinResult;
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

    private ChangeScene_PlayScene _previousHighScore;
    private GetHighScores _getHighScores;
    private bool _backBool;
    private bool _retryBool;
    private int _previousScore;
    private int _score;
    private string _scoreRank;


    private void Start()
    {
        _getHighScores = gameObject.AddComponent<GetHighScores>();
        _previousHighScore = gameObject.AddComponent<ChangeScene_PlayScene>();

        _previousScore = _previousHighScore.previousHighScore;
        _score = _getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
        _scoreRank = _getHighScores.GetRank(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
        _backBool = true;
        _retryBool = true;

        scoreInResult.text = $"{_score}";
        maxCombo.text = ScoreComboCalculator.highCombo.ToString();
        perfectCom.text = ScoreComboCalculator.sumPerfect.ToString();
        goodCom.text = ScoreComboCalculator.sumGood.ToString();
        badCom.text = ScoreComboCalculator.sumBad.ToString();
        missCom.text = ScoreComboCalculator.sumMiss.ToString();
        titleInResult.text = RhythmGamePresenter.musicName;
        difficultyInResult.text = RhythmGamePresenter.dif;
        jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleInResult.text + "_jacket");
        rankinResult.sprite = Resources.Load<Sprite>("Rank/rank_" + _scoreRank);
        previousScore.text = $"{_previousScore}";
        scoreGap.text = _score switch
        {
            var n when n > _previousScore => "+" + $"{n - _previousScore}",
            var n when n < _previousScore => "-" + $"{_previousScore - n}",
            _=> ""
        };
        
        //clear.sprite =Resources.Load<Sprite>()
        //rankDifficulty =
        //colorInResult =
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
      resultMusic.Stop();
    }
}
