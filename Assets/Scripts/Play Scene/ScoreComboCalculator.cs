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
        private float score = 1;

        //List<JudgeResult> _alljudge;
        //List<JudgeResultInHold> _judgeInHold;

        public Text comboText;
        public Text scoreText;

        public void LateUpdate()
        {
            //_judgeInHold = JudgeService.JudgedInHold; // ���������̎󂯓n��
            //_alljudge = JudgeService.AllJudgeType;
            /*foreach (JudgeResult judgeResult in _alljudge)
            {
                switch (judgeResult.resultType)
                {
                    case JudgeResultType.Perfect:
                        currentCombo++;
                        score += 4;
                        sumPerfect++;
                        break;
                    case JudgeResultType.Good:
                        currentCombo++;
                        score += 2;
                        sumGood++;
                        break;
                    case JudgeResultType.Bad:
                        currentCombo++;
                        score += 1;
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
                }*/
            //JudgeService.AllJudgeType.Clear();

            currentScore = (int) Mathf.Floor(1000000 * score / sumScore);
            comboText.text = currentCombo.ToString();
            scoreText.text = currentScore.ToString();
            //Debug.Log(currentScore);
        }

    }
}
