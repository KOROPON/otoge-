using System.Collections;
using UnityEngine;
using Reilas;

public class ChangeScene_PlayScene : MonoBehaviour
{
    public AudioSource song;
    private ClearRankDirector _clearRankDirector;
    static public int previousHighScore;
    
    public static bool playNoticed;
    public static bool playStopped;

    private void Start()
    {
        playStopped = true;
        _clearRankDirector = GameObject.Find("ClearRankDirector").GetComponent<ClearRankDirector>();
    }

    private void Update()
    {
        if (!playNoticed || !playStopped) return;
        playNoticed = false;
        var getHighScores = gameObject.AddComponent<GetHighScores>();
        var scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();

        StartCoroutine(Checking(() =>
        {
            //曲終了時
            //Clear表示
            if (!playStopped) return;
            getHighScores.Awake();
            previousHighScore = getHighScores.GetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif);
            var clear = scoreComboCalculator.clear;
            getHighScores.SetHighScore(RhythmGamePresenter.musicName, RhythmGamePresenter.dif, scoreComboCalculator.currentScore, clear);
            _clearRankDirector.SelectRank(clear);
            scoreComboCalculator.currentCombo = 0;
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
