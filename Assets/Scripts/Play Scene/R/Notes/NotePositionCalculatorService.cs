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

        private static float _gameSpeed;

        public static float firstChartSpeed;
        public static float normalizedSpeed;
        
        public static void CalculateGameSpeed()
        {
            _gameSpeed = PlayerPrefs.HasKey("rate") ? 4 * PlayerPrefs.GetFloat("rate") * PlayerPrefs.GetFloat("rate") : 100;
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

        private static float CalculateZPos(float judgeTime, float noteSpeed, float currentTime)
        {
            var t = currentTime - judgeTime;
            
            return PositionCalculator(t, NoteSpeedCalculator(firstChartSpeed, noteSpeed));
        }
        
        public static float GetPosition(float judgeTime, float currentTime, float noteSpeed)
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

            return normalizedTime < 0 ? highSpeed * t : CalculateZPos(judgeTime, noteSpeed, currentTime);
        }

        public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}