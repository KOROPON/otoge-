using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rhythmium;


namespace Reilas
{
   public class ScoreComboCaliculator : MonoBehaviour
   {

      public static int sumScore;
      public float currentScore;
      public int currentCombo;
      private int score;

      public Text comboText;
      public Text scoreText;
      void Update()
      {
           foreach (JudgeResultType judgetype in JudgeService.alljudgetype)
           {
               if (judgetype == JudgeResultType.Perfect)
               {
                  currentCombo++;
                  score += 4;
               }
               else if (judgetype == JudgeResultType.Good)
               {
                  currentCombo++;
                  score +=2;
               }
               else if (judgetype == JudgeResultType.Bad)
               {
                 currentCombo++;
                 score +=1;
               }
               else
               {
                 currentCombo = 0;
               }
           }
           JudgeService.alljudgetype.Clear();

           currentScore = Mathf.Floor( 1000000 * score / sumScore );
           comboText.text = currentCombo.ToString();
           scoreText.text = currentScore.ToString();
           Debug.Log(currentScore);
      }

  }
}
