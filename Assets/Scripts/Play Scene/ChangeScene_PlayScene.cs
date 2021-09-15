using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rhythmium;

public class ChangeScene_PlayScene : MonoBehaviour
{
    public AudioSource song;
    void Start()
    {
      GetHighScores getHighScores = new GetHighScores();
      StartCoroutine(Checking( ()=>{
        //曲終了時
        getHighScores.SetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif, ScoreComboCaliculator.currentScore);
        getHighScores.GetHighScore();
        SceneManager.LoadScene("ResultScene");
     } ));
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
