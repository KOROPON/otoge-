using System.Collections;
using UnityEngine;
using Reilas;

public class ChangeScene_PlayScene : MonoBehaviour
{
    public AudioSource song;

    public int previousHighScore;
    
    public static bool playNoticed;
    public static bool playStopped;

    private void Start()
    {
        playStopped = true;
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
            previousHighScore = getHighScores.GetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif);
            getHighScores.SetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif, ScoreComboCalculator.currentScore, ScoreComboCalculator.allPerfect,ScoreComboCalculator.fullCombo);
            ScoreComboCalculator.currentCombo = 0;
            Shutter.blChange = "ToR";
            Shutter.blShutterChange = "Close";
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
