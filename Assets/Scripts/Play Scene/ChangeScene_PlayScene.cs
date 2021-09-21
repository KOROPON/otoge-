using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rhythmium;
using Reilas;

public class ChangeScene_PlayScene : MonoBehaviour
{
    public AudioSource song;
    public static bool playNoticed;
    public static bool playStopped;
    void Start()
    {
      playStopped = true;
    }
    void Update()
    {
       if (playNoticed && playStopped)
       {
         playNoticed =false;
         GetHighScores getHighScores = new GetHighScores();

         StartCoroutine(Checking( ()=>{
           //曲終了時
           //Clear表示
           if (playStopped)
           {
             getHighScores.Awake();
             getHighScores.SetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif, ScoreComboCaliculator.currentScore);
             getHighScores.GetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif);
             ScoreComboCaliculator.currentCombo = 0;
             Shutter.blChange = "ToR";
           }
                   } ));
       }

    }
    public delegate void functionType();
    private IEnumerator Checking (functionType callback) {
        while(true) {
            yield return new WaitForFixedUpdate();
            if (!song.isPlaying) {
                callback();
                break;
            }
        }
    }

}
