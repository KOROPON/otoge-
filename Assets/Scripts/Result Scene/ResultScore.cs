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

    void Start()
    {
        if (PlayerPrefs.HasKey("currentScore") == false)
        {
            PlayerPrefs.SetFloat("currentScore", 0f);
        }

        scoreinResult.text = String.Format("{0, 9: 0,000,000}", ScoreComboCaliculator.currentScore);
        maxCombo.text = ScoreComboCaliculator.highCombo.ToString();
        perfectCom.text = ScoreComboCaliculator.sumPerfect.ToString();
        goodCom.text = ScoreComboCaliculator.sumGood.ToString();
        badCom.text = ScoreComboCaliculator.sumBad.ToString();
        missCom.text = ScoreComboCaliculator.sumMiss.ToString();
        titleinResult.text = RhythmGamePresenter.musicname;
        difficultyinResult.text = MusicNumManage.difficulty;
        jackinResult.sprite = Resources.Load<Sprite>("Jacket/" + titleinResult.text + "_jacket");
        rankinResult.sprite = Resources.Load<Sprite>("Rank/score_" + ScoreComboCaliculator.scoreRank); //まだやってない
        previousScore.text = String.Format("{0, 9: 0,000,000}", PlayerPrefs.GetFloat("previousScore"));
        scoreGap.text = PlayerPrefs.GetFloat("previousScore") <= ScoreComboCaliculator.currentScore
            ? "+" + String.Format("{0, 9: 0,000,000}",
                ScoreComboCaliculator.currentScore - PlayerPrefs.GetFloat("previousScore"))
            : "-" + String.Format("{0, 9: 0,000,000}",
                PlayerPrefs.GetFloat("previousScore") - ScoreComboCaliculator.currentScore);
        if (PlayerPrefs.HasKey("currentScore") == false ||
            ScoreComboCaliculator.currentScore > PlayerPrefs.GetFloat("previousScore"))
        {
            PlayerPrefs.SetFloat("currentScore", ScoreComboCaliculator.currentScore);
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