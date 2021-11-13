using System.Collections.Generic;
using UnityEngine;
using Rhythmium;

namespace Reilas
{
    public static class PositionCalculator
    {
        private static float CalculateZPos(float judgeTime, List<SpeedChangeEntity> speedChangeEntities, float currentTime)
        {
            var t = currentTime - judgeTime;
            if (speedChangeEntities.Count == 0)
                return NotePositionCalculatorService.PositionCalculator(t,
                    NotePositionCalculatorService.SpeedCalculator(NotePositionCalculatorService.firstChartSpeed));
            var zPos = 0f;
            for (var i = speedChangeEntities.Count - 1; i >= 0; i--)
            {
                var nextNotePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i + 1);
                if (currentTime >= nextNotePassedTime) break;
                var notePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i);
                if (judgeTime < notePassedTime) continue;
                var highSpeed = NotePositionCalculatorService.SpeedCalculator(speedChangeEntities[i].Speed);
                var nextNotePosition =
                    NotePositionCalculatorService.PositionCalculator(nextNotePassedTime - judgeTime, highSpeed);
                zPos += currentTime < RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i - 1)
                    ? NotePositionCalculatorService.PositionCalculator(t, highSpeed) - nextNotePosition
                    : NotePositionCalculatorService.PositionCalculator(notePassedTime - judgeTime, highSpeed) -
                      nextNotePosition;
            }

            return zPos;
        }
       
        private static Vector3 CalculatePosition(float judgeTime, float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            var highSpeed = NotePositionCalculatorService.normalizedSpeed;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - judgeTime;
            var normalizedTime = -t * highSpeed / 600f;

            return normalizedTime switch
            {
                var time when time >= 1 => new Vector3(0f, 0f, 999f),
                var time when time >= 0 => new Vector3(0f, 0f, CalculateZPos(judgeTime, speedChangeEntities, currentTime)),
                _ => new Vector3(0f, 0f, 0f)
            };
        }
    }
}