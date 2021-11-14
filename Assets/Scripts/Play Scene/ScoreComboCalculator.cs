using System;
using System.Collections.Generic;
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
        public string clear;
        public Image slider;

        private Sprite nomalGauge;
        private Sprite bossGauge;

        private float _sumScore;
        private float _score;
        private int _gaugeCombo;
        private int _gaugeMiss;
        private string _difficulty;
        private float _value;
        
        private readonly Dictionary<string, int> _comboDataBase = new Dictionary<string, int>()
        {
            {"Easy", 2},
            {"Hard", 4},
            {"Extreme", 7}
        };

        public Text comboText;
        public Text gauge;

        private void Start()
        {
            _difficulty = PlayerPrefs.GetString("difficulty");
            slider = GameObject.Find("Fill").GetComponent<Image>();

            nomalGauge = Resources.Load<Sprite>("Gauge/GaugeFill") as Sprite;
            bossGauge = Resources.Load<Sprite>("Gauge/HardGaugeFill") as Sprite;


            sumPerfect = 0; 
            sumGood = 0;
            sumBad = 0;
            sumMiss = 0;
            currentCombo = 0;
            highCombo = 0;
            _sumScore = 0;
            currentScore = 0;
            _score = 0;
            
            slider.fillAmount = 0f;
            _gaugeCombo = 0;
            _gaugeMiss = 0;
            _sumScore = RhythmGamePresenter.countNotes * 4;
            
            comboText.text = "";
            //gauge.text = "0";
        }

        public void LateUpdate()
        {
            foreach (var judgeResult in AllJudgeService.AllJudge)
            {
                switch (judgeResult)
                {
                    case JudgeResultType.Perfect:
                        {
                            currentCombo++;
                             _score += 4;
                            sumPerfect++;
                            _gaugeCombo++;
                            _gaugeMiss = 0;
                            break;
                        }
                    case JudgeResultType.Good:
                    {
                        currentCombo++;
                        _score += 2;
                        sumGood++;
                        _gaugeCombo++;
                        _gaugeMiss = 0;
                        break;
                    }
                    case JudgeResultType.Bad:
                    {
                        currentCombo++;
                        _score += 1;
                        sumBad++;
                        _gaugeCombo++;
                        _gaugeMiss = 0;
                        break;
                    }
                    case JudgeResultType.Miss:
                    {
                        if (highCombo < currentCombo) highCombo = currentCombo;
                        currentCombo = 0;
                        _gaugeCombo = 0;
                        _gaugeMiss++;
                        sumMiss++;
                        break;
                    }
                    case JudgeResultType.NotJudgedYet:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            AllJudgeService.AllJudge.Clear();

            currentScore = (int) Mathf.Floor(1000000 * _score / _sumScore);
            comboText.text = currentCombo > 1 ? currentCombo.ToString() : "";

            while (_gaugeCombo >= _comboDataBase[_difficulty])
            {
                if(slider.fillAmount <= 0.99) slider.fillAmount += 0.01f;
                _gaugeCombo -= _comboDataBase[_difficulty];
            }

            if(slider.fillAmount >= 0.01) slider.fillAmount -= 0.01f * _gaugeMiss;

            //gauge.text = slider.value.ToString(CultureInfo.InvariantCulture);
            if (currentScore == 1000000) clear = "AllPerfect";
            else if (currentCombo == RhythmGamePresenter.countNotes) clear = "FullCombo";
            else if (_value >= 0.7f) clear = "Clear";
            else clear = "Failed";
        }

        public void GaugeChange()
        {
            slider.sprite = bossGauge;
        }
    }
}
