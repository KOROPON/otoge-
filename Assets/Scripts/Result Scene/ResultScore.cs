using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Reilas;

public class ResultScore : MonoBehaviour
{
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
  public Image rankinResult;
  public Image colorinResult;

  void Start()
  {
    scoreinResult.text = String.Format("{0, 9: 0,000,000}", ScoreComboCaliculator.currentScore);
    maxCombo.text = ScoreComboCaliculator.highCombo.ToString();
    perfectCom.text = ScoreComboCaliculator.sumPerfect.ToString();
    goodCom.text = ScoreComboCaliculator.sumGood.ToString();
    badCom.text = ScoreComboCaliculator.sumBad.ToString();
    missCom.text = ScoreComboCaliculator.sumMiss.ToString();
    titleinResult.text = "Collide";//RhythmGamePresenter.musicname;
    difficultyinResult.text =MusicNumManage.difficulty;
    jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleinResult.text + "_jacket");
    rankinResult.sprite = Resources.Load<Sprite>("Rank/score_"+ScoreComboCaliculator.scoreRank);
    //rankDifficulty =
    //colorinResult =
    //未実装

    ScoreComboCaliculator.currentScore = 0;
    ScoreComboCaliculator.highCombo = 0;
    ScoreComboCaliculator.sumPerfect = 0;
    ScoreComboCaliculator.sumGood = 0;
    ScoreComboCaliculator.sumBad = 0;
    ScoreComboCaliculator.sumMiss = 0;
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
