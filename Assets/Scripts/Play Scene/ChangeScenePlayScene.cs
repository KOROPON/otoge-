using System.Collections;
using UnityEngine;
using ShutterScene;
using Reilas;

public class ChangeScenePlayScene : MonoBehaviour
{
    public AudioSource song;
    
    private ClearRankDirector _clearRankDirector;
    private ScoreComboCalculator _scoreComboCalculator;
    private GetHighScores _getHighScores;
    
    private bool _tutorial;
    
    public static int previousHighScore;
    public static int score;
    public static string clear;

    public static bool playNoticed;
    public static bool playStopped;

    public bool forcedFinish;

    private void Start()
    {
        playStopped = true;
        forcedFinish = false;
        _clearRankDirector = GameObject.Find("ClearRankDirector").GetComponent<ClearRankDirector>();
        _scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
        _getHighScores = GetComponent<GetHighScores>();
    }

    public void Update()
    {
        if (forcedFinish!)
        {
            Debug.Log("bossGauge");
            CallBack();
            forcedFinish = false;
        }
        if (!playNoticed || !playStopped) return;
        playNoticed = false;

        StartCoroutine(Checking(CallBack));
    }
    private void CallBack()
    {
        if (!playStopped) return;
        if (RhythmGamePresenter.tutorial)
        {
            Shutter.blChange = "ToSFrP";
            Shutter.blShutterChange = "Close";
            RhythmGamePresenter.tutorial = false;
            PlayerPrefs.SetInt("tutorialDebug15", 1); //int�^�̒l(1)�ŕۑ�
            PlayerPrefs.Save();
            return;
        }

        SettingField.setBool = false;
        
        _getHighScores.Awake();
        
        previousHighScore = _getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
        score = _scoreComboCalculator.currentScore;
        
        if (ScoreComboCalculator.highCombo < _scoreComboCalculator.currentCombo)
            ScoreComboCalculator.highCombo = _scoreComboCalculator.currentCombo;
        
        clear = _scoreComboCalculator.clear;
        _clearRankDirector.SelectRank(clear);
        _scoreComboCalculator.currentCombo = 0;
        if (score > previousHighScore && !RhythmGamePresenter.jumpToKujo)
            _getHighScores.SetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif, score, clear);
    }

    public void AwakeCallBack()
    {
        CallBack();
    }

    private delegate void FunctionType();

    private IEnumerator Checking(FunctionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (song.isPlaying) continue;
            callback();
            break;
        }
    }
}
