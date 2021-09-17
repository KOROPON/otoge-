using System;
using Reilas;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    public Text scoreinResult;
    public Text previousScore;
    public Text scoreGap;
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
    public Image clear;
    private string scoreRank;
    private float score;

    void Start()
    {

        score = ScoreComboCaliculator.currentScore;
        switch (score)
        {
          //case int n when n >= 995000: scoreRank = "SSS"; break;
          case float n when n >= 990000: scoreRank = "SS"; break;
          case float n when n >= 980000: scoreRank = "S"; break;
          case float n when n >= 950000: scoreRank = "A"; break;
          case float n when n >= 900000: scoreRank = "B"; break;
          case float n when n >= 800000: scoreRank = "C"; break;
          case float n when n < 800000: scoreRank = "D";break;
        }
        if (PlayerPrefs.HasKey("currentScore") == false)
        {
            PlayerPrefs.SetFloat("currentScore", 0f);
        }
        scoreinResult.text = String.Format("{0, 9: 0,000,000}",(float) score);
        maxCombo.text = ScoreComboCaliculator.highCombo.ToString();
        perfectCom.text = ScoreComboCaliculator.sumPerfect.ToString();
        goodCom.text = ScoreComboCaliculator.sumGood.ToString();
        badCom.text = ScoreComboCaliculator.sumBad.ToString();
        missCom.text = ScoreComboCaliculator.sumMiss.ToString();
        titleinResult.text = RhythmGamePresenter.musicname;
        difficultyinResult.text = RhythmGamePresenter.dif;
        jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleinResult.text + "_jacket");
        rankinResult.sprite = Resources.Load<Sprite>("Rank/score_" + scoreRank);
        previousScore.text = String.Format("{0, 9: 0,000,000}", PlayerPrefs.GetFloat("currentScore"));
        scoreGap.text = PlayerPrefs.GetFloat("currentScore") <= score
            ? "+" + String.Format("{0, 9: 0,000,000}",
                score - PlayerPrefs.GetFloat("currentScore"))
            : "-" + String.Format("{0, 9: 0,000,000}",
                PlayerPrefs.GetFloat("currentScore") - score);
        if (score > PlayerPrefs.GetFloat("currentScore"))
        {
            PlayerPrefs.SetFloat("currentScore", score);
        }
        //clear.sprite =Resources.Load<Sprite>()
        //rankDifficulty =
        //colorinResult =
        //未実装00

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
