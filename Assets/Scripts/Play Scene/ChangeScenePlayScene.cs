using System.Collections;
using UnityEngine;
using Reilas;

public class ChangeScenePlayScene : MonoBehaviour
{
    public AudioSource song;
    private ClearRankDirector _clearRankDirector;
    private ScoreComboCalculator scoreComboCalculator;
    private GetHighScores getHighScores;
    static public int previousHighScore;
    static public int score;
    static public string clear;

    public static bool playNoticed;
    public static bool playStopped;

    public bool forcedFinish;

    private void Start()
    {
        playStopped = true;
        forcedFinish = false;
        _clearRankDirector = GameObject.Find("ClearRankDirector").GetComponent<ClearRankDirector>();
        scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
        getHighScores = GetComponent<GetHighScores>();
    }

    public void Update()
    {
        if (forcedFinish)
        {
            CallBack();
            forcedFinish = false;
        }
        if (!playNoticed || !playStopped) return;
        playNoticed = false;

        StartCoroutine(Checking(() =>CallBack()));
        void CallBack()
        {
            //ã»èIóπéû
            //Clearï\é¶
            if (!playStopped) return;
            SettingField.setBool = false;
            getHighScores.Awake();
            previousHighScore = getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
            score = scoreComboCalculator.currentScore;
            if (ScoreComboCalculator.highCombo < scoreComboCalculator.currentCombo)
            {
                ScoreComboCalculator.highCombo = scoreComboCalculator.currentCombo;
            }
            clear = scoreComboCalculator.clear;
            getHighScores.SetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif, score, clear);
            _clearRankDirector.SelectRank(clear);
            scoreComboCalculator.currentCombo = 0;
        }
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
