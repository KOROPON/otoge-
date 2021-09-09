using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reilas
{
    public class ScoreComboCaliculator : MonoBehaviour
    {

        public static int sumScore = 1;
        public float currentScore;
        public int currentCombo;
        private int score = 1;

        List<JudgeResult> alljudge;
        List<JudgeResultInHold> judgeInHold;

        public Text comboText;
        public Text scoreText;
        void LateUpdate()
        {
            judgeInHold = JudgeService.judgedInHold; // “à•””»’è‚ÌŽó‚¯“n‚µ
            alljudge = JudgeService.allJudgeType;
            foreach (JudgeResult judgeResult in alljudge)
            {
                var judgetype = judgeResult.ResultType;
                if (judgetype == JudgeResultType.Perfect)
                {
                    currentCombo++;
                    score += 4;
                }
                else if (judgetype == JudgeResultType.Good)
                {
                    currentCombo++;
                    score += 2;
                }
                else if (judgetype == JudgeResultType.Bad)
                {
                    currentCombo++;
                    score += 1;
                }
                else
                {
                    currentCombo = 0;
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
