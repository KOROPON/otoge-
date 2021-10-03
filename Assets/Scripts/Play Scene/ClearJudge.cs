using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reilas;

public class ClearJudge : MonoBehaviour
{
   private int splitting;
   private int gageCom;
   private int gagepoint = 0;
   void Start()
   {
     switch(RhythmGamePresenter.dif)
     {
       case "Extreme": splitting = 7; break;
       case "Hard": splitting = 4; break;
       case "Easy": splitting = 2; break;
     }
     gagepoint = 0;
     gageCom = 0;
   }
   void Update()
   {
     if (gagepoint < (int) Mathf.Floor(ScoreComboCalculator.currentCombo / splitting))
     {
       gageCom++;
       gagepoint++;
     }
     if (gagepoint > (int) Mathf.Floor(ScoreComboCalculator.currentCombo / splitting))
     {
       gageCom += (-3);
       gagepoint = 0;
     }
     Debug.Log(gageCom);
   }
}
