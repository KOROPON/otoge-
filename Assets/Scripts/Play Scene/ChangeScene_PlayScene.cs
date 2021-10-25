using System.Collections;
using UnityEngine;
using Reilas;

public class ChangeScene_PlayScene : MonoBehaviour
{
    private ScoreComboCalculator _scoreComboCalculator;
    
    public AudioSource song;
    public ClearRankDirector clearRankDirector;
    public int previousHighScore;
    
    public static bool playNoticed;
    public static bool playStopped;

    private void Start()
    {
        playStopped = true;
        _scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
    }

    private void Update()
    {
        if (!playNoticed || !playStopped) return;
        playNoticed = false;
        var getHighScores = gameObject.AddComponent<GetHighScores>();

        StartCoroutine(Checking(() =>
        {
            //曲終了時
            //Clear表示
            if (!playStopped) return;
            getHighScores.Awake();
            previousHighScore = getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
            getHighScores.SetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif, _scoreComboCalculator.currentScore, _scoreComboCalculator.allPerfect, _scoreComboCalculator.fullCombo, _scoreComboCalculator.slider.value >= 0.7f);
            _scoreComboCalculator.currentCombo = 0;
            Shutter.blChange = "ToR";
            Shutter.blShutterChange = "Close";
            _scoreComboCalculator.currentCombo = 0;
            clearRankDirector.SelectRank(getHighScores.GetRank(RhythmGamePresenter.musicName, RhythmGamePresenter.dif));
        }));

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
