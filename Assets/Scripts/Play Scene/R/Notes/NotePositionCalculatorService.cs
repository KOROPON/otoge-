#nullable enable

using Rhythmium;
using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    /// <summary>
    /// ノーツの位置を計算するサービス
    /// </summary>
    public static class NotePositionCalculatorService
    {
        private const float BelowNoteWidth = 2.2f;
        private const float LeftPosition = -4.4f;

        private static float _gameSpeed;

        public static float firstChartSpeed;
        public static float normalizedSpeed;
        
        public static void CalculateGameSpeed()
        {
            _gameSpeed = PlayerPrefs.HasKey("rate") ? 10 * PlayerPrefs.GetFloat("rate") : 10;
        }

        public static float SpeedCalculator(float bpm)
        {
            return _gameSpeed * bpm * 0.01f;
        }
        
        public static void CalculateNoteSpeed(float noteSpeed)
        {
            normalizedSpeed = SpeedCalculator(noteSpeed);
        }
        
        public static float PositionCalculator(float span, float speed)
        {
            return speed * 0.5f * span * span - speed * span;
        }

        private static float NoteSpeedCalculator(float bpm, float noteSpeed)
        {
            return SpeedCalculator(bpm) * noteSpeed;
        }

        private static float CalculateZPos(float judgeTime, List<SpeedChangeEntity> speedChangeEntities, float noteSpeed, float currentTime)
        {
            var t = currentTime - judgeTime;
            
            if (speedChangeEntities.Count == 0) return PositionCalculator(t, NoteSpeedCalculator(firstChartSpeed, noteSpeed));
            
            var zPos = 0f;
            
            for (var i = speedChangeEntities.Count - 1; i >= 0; i--)
            {
                var nextNotePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i + 1);
                var notePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i);
                
                var timeCheck = currentTime >= nextNotePassedTime;
                var beforeTimeCheck = judgeTime < notePassedTime;
                
                if (i == speedChangeEntities.Count - 1 && timeCheck || i == 0 && beforeTimeCheck)
                    return PositionCalculator(t, NoteSpeedCalculator(speedChangeEntities[i].Speed, noteSpeed));
                
                if (timeCheck) break;
                if (beforeTimeCheck) continue;
                
                var highSpeed = NoteSpeedCalculator(speedChangeEntities[i].Speed, noteSpeed);

                var judgeTimePosition = nextNotePassedTime - judgeTime;
                var nextNotePosition = judgeTimePosition > 0f ? 0f : PositionCalculator(judgeTimePosition, highSpeed);
                var positionCalculator = currentTime > notePassedTime
                    ? PositionCalculator(t, highSpeed)
                    : PositionCalculator(notePassedTime - judgeTime, highSpeed);
                
                zPos += positionCalculator - nextNotePosition;
            }

            return zPos;
        }
        
        public static float GetPosition(float judgeTime, float currentTime, float noteSpeed, List<SpeedChangeEntity> speedChangeEntities)
        {
            var highSpeed = normalizedSpeed * noteSpeed;

            //var size = entity.Size * BelowNoteWidth;
            //var left = size / 2f;
            //var pos = LeftPosition + left;

            //pos += entity.LanePosition * BelowNoteWidth;

            //var toLeft = left - LeftPosition;

            //var x = pos;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = judgeTime - currentTime;
            var normalizedTime = t * _gameSpeed / 600f;

            return normalizedTime < 0 ? highSpeed * t : CalculateZPos(judgeTime, speedChangeEntities, noteSpeed, currentTime);
        }

        public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}