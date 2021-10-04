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
        public static int currentCombo;
        public static int highCombo=0;
        public float sumScore = 1 * 4;　//総コンボ数　* 4;
        public static　int currentScore = 0;
        private float _score = 1;

        //List<JudgeResult> _alljudge;
        //List<JudgeResultInHold> _judgeInHold;

        public Text comboText;
        public Text scoreText;

        public void LateUpdate()
        {
            foreach (JudgeResultType judgeResult in JudgeService.AllJudge)
            {
                switch (judgeResult)
                {
                    case JudgeResultType.Perfect:
                        currentCombo++;
                        _score += 4;
                        sumPerfect++;
                        Gauge.combo++;
                        Gauge.miss = 0;
                        break;
                    case JudgeResultType.Good:
                        currentCombo++;
                        _score += 2;
                        sumGood++;
                        Gauge.combo++;
                        Gauge.miss = 0;
                        break;
                    case JudgeResultType.Bad:
                        currentCombo++;
                        _score += 1;
                        sumBad++;
                        Gauge.combo++;
                        Gauge.miss = 0;
                        break;
                    case JudgeResultType.Miss:
                        if (highCombo < currentCombo)
                        {
                            highCombo = currentCombo;
                        }

                        currentCombo = 0;
                        Gauge.combo = 0;
                        Gauge.miss++;
                        sumMiss++;
                        break;
                }
                
                JudgeService.AllJudge.Clear();

                currentScore = (int) Mathf.Floor(1000000 * _score / sumScore);
                comboText.text = currentCombo.ToString();
                scoreText.text = currentScore.ToString();
                //Debug.Log(currentScore);
            }
        }
    }
}
