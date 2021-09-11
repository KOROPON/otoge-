using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reilas
{
    public class ScoreComboCaliculator : MonoBehaviour
    {

        public static int sumPerfect;
        public static int sumGood;
        public static int sumBad;
        public static int sumMiss;
        public int currentCombo;
        public static int highCombo=0;
        public float sumScore = 1 * 4;　
        public static　float currentScore;
        private float score = 1;

        List<JudgeResult> alljudge;
        List<JudgeResultInHold> judgeInHold;

        public Text comboText;
        public Text scoreText;
        void LateUpdate()
        {
            judgeInHold = JudgeService.judgedInHold; // ���������̎󂯓n��
            alljudge = JudgeService.allJudgeType;
            foreach (JudgeResult judgeResult in alljudge)
            {
                var judgetype = judgeResult.ResultType;
                if (judgetype == JudgeResultType.Perfect)
                {
                    currentCombo++;
                    score += 4;
                    sumPerfect++;
                }
                else if (judgetype == JudgeResultType.Good)
                {
                    currentCombo++;
                    score += 2;
                    sumGood++;
                }
                else if (judgetype == JudgeResultType.Bad)
                {
                    currentCombo++;
                    score += 1;
                    sumBad++;
                }
                else
                {
                    if (highCombo < currentCombo)
                    {
                      highCombo = currentCombo;
                    }
                    currentCombo = 0;
                    sumMiss++;
                }
            }
            JudgeService.allJudgeType.Clear();

            currentScore = Mathf.Floor(1000000 * score / sumScore);
            comboText.text = currentCombo.ToString();
            scoreText.text = currentScore.ToString();
            //Debug.Log(currentScore);
        }

    }
}
