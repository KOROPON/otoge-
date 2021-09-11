using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Reilas;

public class ResultScore{
  public Text scoreinResult;
  public Text maxCombo;
  public Text perfectCom;
  public Text goodCom;
  public Text badCom;
  public Text missCom;
  public Text titleinResult;
  public Text difficultyinResult;
  public Text rankDifficulty;
  public Image jackinResult;
  public Image colorinResult;

  void Start()
  {
    scoreinResult.text = ScoreComboCaliculator.currentScore.ToString();
    maxCombo.text = ScoreComboCaliculator.highCombo.ToString();
    perfectCom.text = ScoreComboCaliculator.sumPerfect.ToString();
    goodCom.text = ScoreComboCaliculator.sumGood.ToString();
    badCom.text = ScoreComboCaliculator.sumBad.ToString();
    missCom.text = ScoreComboCaliculator.sumMiss.ToString();
    titleinResult.text = RhythmGamePresenter.musicname.ToString();
    difficultyinResult.text =MusicNumManage.difficulty.ToString();
    jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleinResult.text + "_jacket");



  }

  public void Back()
  {
    SceneManager.LoadScene("SelectScene");
  }

  public void Retry()
  {

    SceneManager.LoadScene("PlayScene");
  }
}
