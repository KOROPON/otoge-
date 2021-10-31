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

        public static void CalculateNoteSpeed(float noteSpeed)
        {
            normalizedSpeed = _gameSpeed * noteSpeed * 0.01f;
        }

        public static float SpeedCalculator(float bpm)
        {
            return _gameSpeed * bpm * 0.01f;
        }
        
        public static float PositionCalculator(float span, float speed)
        {
            return speed * 0.5f * span * span - speed * span;
        }

        private static float NoteSpeedCalculator(float bpm, float noteSpeed)
        {
            return SpeedCalculator(bpm) * noteSpeed;
        }

        private static float CalculateZPos(NoteEntity entity, List<SpeedChangeEntity> speedChangeEntities, float noteSpeed, float currentTime)
        {
            var judgeTime = entity.JudgeTime;
            var t = currentTime - judgeTime;
            if (speedChangeEntities.Count == 0) return PositionCalculator(t, NoteSpeedCalculator(firstChartSpeed, noteSpeed));
            var zPos = 0f;
            for (var i = speedChangeEntities.Count - 1; i >= 0; i--)
            {
                var nextNotePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i + 1);
                if (currentTime >= nextNotePassedTime) break;
                var notePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i);
                if (judgeTime < notePassedTime) continue;
                var highSpeed = NoteSpeedCalculator(speedChangeEntities[i].Speed, noteSpeed);
                var nextNotePosition = PositionCalculator(nextNotePassedTime - judgeTime, highSpeed);
                zPos += currentTime < RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i - 1)
                    ? PositionCalculator(t, highSpeed) - nextNotePosition
                    : PositionCalculator(notePassedTime - judgeTime, highSpeed) -
                      nextNotePosition;
            }

            return zPos;
        }
        
        public static Vector3 GetPosition(NoteEntity entity, float currentTime, float noteSpeed, bool checkIfTap, List<SpeedChangeEntity> speedChangeEntities)
        {
            var highSpeed = normalizedSpeed * noteSpeed;

            var size = entity.Size * BelowNoteWidth;
            var left = size / 2f;
            var pos = LeftPosition + left;

            Vector3 notePos;

            pos += entity.LanePosition * BelowNoteWidth;

            //var toLeft = left - LeftPosition;

            var x = pos;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var judgeTime = entity.JudgeTime;
            var t = currentTime - judgeTime;
            var normalizedTime = -t * _gameSpeed / 600f;

            if (normalizedTime < 0) return new Vector3(x, 0f, -highSpeed * t);
            if (!checkIfTap)
                return new Vector3(x, 0f, CalculateZPos(entity, speedChangeEntities, noteSpeed, currentTime));
            return normalizedTime switch
            {
                var n when n >= 1 => new Vector3(0f, 0f, 999f),
                _ => new Vector3(x, 0f, CalculateZPos(entity, speedChangeEntities, noteSpeed, currentTime))
            };
        }

            public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}