using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Reilas
{
    public class ScoreComboCalculator : MonoBehaviour
    {
        public static int sumPerfect;
        public static int sumGood;
        public static int sumBad;
        public static int sumMiss;
        public static int highCombo;
        
        public int currentCombo;
        public int currentScore;
        public Slider slider;
        public string clear;
        
        private float _sumScore;
        private float _score;
        private int _gaugeCombo;
        private int _gaugeMiss;
        private string _difficulty;
        
        private readonly Dictionary<string, int> _comboDataBase = new Dictionary<string, int>()
        {
            {"Easy", 2},
            {"Hard", 4},
            {"Extreme", 7}
        };

        public Text comboText;
        public Text scoreText;
        public Text gauge;

        private void Start()
        {
            _difficulty = PlayerPrefs.GetString("difficulty");
            slider = GameObject.Find("ScoreGauge").GetComponent<Slider>();
            
            sumPerfect = 0; 
            sumGood = 0;
            sumBad = 0;
            sumMiss = 0;
            currentCombo = 0;
            highCombo = 0;
            _sumScore = 0;
            currentScore = 0;
            _score = 0;
            
            slider.value = 0f;
            _gaugeCombo = 0;
            _gaugeMiss = 0;
            _sumScore = RhythmGamePresenter.countNotes * 4;
            
            comboText.text = "";
            scoreText.text = "0,000,000";
            gauge.text = "0";
        }

        public void LateUpdate()
        {
            foreach (var judgeResult in JudgeService.AllJudge)
            {
                switch (judgeResult)
                {
                    case JudgeResultType.Perfect:
                        currentCombo++;
                        _score += 4;
                        sumPerfect++;
                        _gaugeCombo++;
                        _gaugeMiss = 0;
                        break;
                    case JudgeResultType.Good:
                        currentCombo++;
                        _score += 2;
                        sumGood++;
                        _gaugeCombo++;
                        _gaugeMiss = 0;
                        break;
                    case JudgeResultType.Bad:
                        currentCombo++;
                        _score += 1;
                        sumBad++;
                        _gaugeCombo++;
                        _gaugeMiss = 0;
                        break;
                    case JudgeResultType.Miss:
                        if (highCombo < currentCombo)
                        {
                            highCombo = currentCombo;
                        }

                        currentCombo = 0;
                        _gaugeCombo = 0;
                        _gaugeMiss++;
                        sumMiss++;
                        break;
                    case JudgeResultType.NotJudgedYet:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            JudgeService.AllJudge.Clear();

            currentScore = (int) Mathf.Floor(1000000 * _score / _sumScore);
            comboText.text = currentCombo > 1 ? currentCombo.ToString() : "";
            scoreText.text = $"{currentScore,9: 0,000,000}";
            
            while (_gaugeCombo >= _comboDataBase[_difficulty])
            {
                slider.value += 0.01f;
                _gaugeCombo -= _comboDataBase[_difficulty];
            }

            slider.value -= 0.03f * _gaugeMiss;

            gauge.text = slider.value.ToString(CultureInfo.InvariantCulture);
            
            if (currentScore == 1000000) clear = "AllPerfect";
            else if (currentCombo == RhythmGamePresenter.countNotes) clear = "FullCombo";
            else if (slider.value >= 0.7f) clear = "Clear";
            else clear = "Failed";
        }
    }
}
