using UnityEngine;
using UnityEngine.UI;
using Reilas;

public class ScoreBoard : MonoBehaviour
{
    private GetHighScores _getHighScores;
    private ScoreComboCalculator _scoreComboCalculator;
    private Image _scoreBoard;
    private Text _name;
    private Text _difficulty;
    private Text _figureDifficulty;
    private Image _jacket;
    private Text _score;

    private void Start()
    {
        _getHighScores = GameObject.Find("Main").GetComponent<GetHighScores>();
        _scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
        _scoreBoard = GetComponent<Image>();
        _name = GetComponentsInChildren<Text>()[1];
        _difficulty = GetComponentsInChildren<Text>()[2];
        _figureDifficulty = GetComponentsInChildren<Text>()[3];
        _jacket = GetComponentsInChildren<Image>()[1];
        _score = GetComponentsInChildren<Text>()[0];

        _scoreBoard.sprite = Resources.Load<Sprite>("ScoreBoard/" + RhythmGamePresenter.dif);
        _name.text = RhythmGamePresenter.musicName;
        _difficulty.text = RhythmGamePresenter.dif;
        _figureDifficulty.text = LevelConverter.GetLevel(RhythmGamePresenter.musicName, RhythmGamePresenter.dif).ToString();
        _jacket.sprite = Resources.Load<Sprite>("Jacket/" + RhythmGamePresenter.musicName + "_jacket");
        _score.text = "0,000,000";
        switch (RhythmGamePresenter.dif)
        {
            case "Easy": _difficulty.color = new Color32(9, 135, 128, 255); _figureDifficulty.color = new Color32(9, 135, 128, 255);break;
            case "Hard": _difficulty.color = new Color32(135, 133, 9, 255); _figureDifficulty.color = new Color32(135, 133, 9, 255); break;
            case "Extreme": _difficulty.color = new Color32(120, 9, 135, 255); _figureDifficulty.color = new Color32(120, 9, 135, 255); break;
            case "Kujo": _difficulty.color = new Color32(150, 150, 150, 255); _figureDifficulty.color = new Color32(150, 150, 150, 255); break;
        }
    }

    private void Update()
    {
        _score.text = $"{_scoreComboCalculator.currentScore,9: 0,000,000}";
    }
}
