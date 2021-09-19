using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rhythmium;
using Reilas;

public class ChangeScene_PlayScene : MonoBehaviour
{
    public AudioSource song;
    void Start()
    {
        
      GetHighScores getHighScores = new GetHighScores();
      StartCoroutine(Checking( ()=>{
        //曲終了時
        //Clear表示
        getHighScores.Awake();
        getHighScores.SetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif, ScoreComboCaliculator.currentScore);
        //シャッター閉じる
        getHighScores.GetHighScore(RhythmGamePresenter.musicname, RhythmGamePresenter.dif);
        ScoreComboCaliculator.currentCombo = 0;
        SceneManager.LoadScene("ResultScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
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
