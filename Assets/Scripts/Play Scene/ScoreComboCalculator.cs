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
            foreach (JudgeResultType judgeResult in JudgeService.allJudge)
            {
                switch (judgeResult)
                {
                    case JudgeResultType.Perfect:
                        currentCombo++;
                        _score += 4;
                        sumPerfect++;
                        break;
                    case JudgeResultType.Good:
                        currentCombo++;
                        _score += 2;
                        sumGood++;
                        break;
                    case JudgeResultType.Bad:
                        currentCombo++;
                        _score += 1;
                        sumBad++;
                        break;
                    case JudgeResultType.Miss:
                        if (highCombo < currentCombo)
                        {
                            highCombo = currentCombo;
                        }

                        currentCombo = 0;
                        sumMiss++;
                        break;
                }
            //JudgeService.AllJudgeType.Clear();

            currentScore = (int) Mathf.Floor(1000000 * _score / sumScore);
            comboText.text = currentCombo.ToString();
            scoreText.text = currentScore.ToString();
            //Debug.Log(currentScore);
        }

    }
}
