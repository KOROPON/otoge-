#nullable enable

using System;
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
        private static float _normalizedSpeed;

        public static float firstChartSpeed;
        
        public static void CalculateGameSpeed()
        {
            _gameSpeed = PlayerPrefs.HasKey("rate") ? 10 * PlayerPrefs.GetFloat("rate") : 10;
        }

        private static float SpeedCalculator(float bpm)
        {
            return _gameSpeed * bpm / firstChartSpeed;
        }
        
        public static void CalculateNoteSpeed(float noteSpeed)
        {
            _normalizedSpeed = SpeedCalculator(noteSpeed);
        }
        
        private static float PositionCalculator(float span, float speed)
        {
            return span * speed * (0.5f * span - 1f);
        }

        private static float NoteSpeedCalculator(float bpm, float noteSpeed)
        {
            return SpeedCalculator(bpm) * noteSpeed;
        }

        public static float SpanCalculator(int index, float judgeTime, float noteSpeed)
        {
            var difference = RhythmGamePresenter.CalculatePassedTime(index) - judgeTime;
            var speedChanges = RhythmGamePresenter.SpeedChanges;
            var changingSpeed = NoteSpeedCalculator(speedChanges[index].Speed, noteSpeed);

            if (index == speedChanges.Count - 1) return 0;

            if (index != 0)
                return PositionCalculator(difference, changingSpeed) -
                       PositionCalculator(RhythmGamePresenter.CalculatePassedTime(index + 1) - judgeTime,
                           changingSpeed);
            
            var firstSpeed = NoteSpeedCalculator(firstChartSpeed, noteSpeed);
            
            return PositionCalculator(-judgeTime, firstSpeed) - PositionCalculator(difference, firstSpeed);
        }

        public static float LeftOverPositionCalculator(float judgeTime, float speed)
        {
            var position = 0f;
            var speedChanges = RhythmGamePresenter.SpeedChanges;
            var zeroPos = PositionCalculator(-judgeTime, NoteSpeedCalculator(firstChartSpeed, speed));

            if (!RhythmGamePresenter.checkSpeedChangeEntity)
            {
                position += zeroPos;
                return position;
            }
            
            for (var i = 0; i < speedChanges.Count; i++)
            {
                var passedTime = RhythmGamePresenter.CalculatePassedTime(i);
                var checkI = i == 0;

                if (judgeTime < passedTime) 
                {
                    var beforeIndex = i - 1;
                    position += checkI
                        ? zeroPos
                        : PositionCalculator(RhythmGamePresenter.CalculatePassedTime(beforeIndex) - judgeTime,
                            NoteSpeedCalculator(speedChanges[beforeIndex].Speed, speed));
                    break;
                }

                if (i == 0)
                    position += zeroPos - PositionCalculator(passedTime - judgeTime,
                        NoteSpeedCalculator(firstChartSpeed, speed));
                
                position += SpanCalculator(i, judgeTime, speed);
            }

            return position;
        }
        
        public static float GetPosition(float judgeTime, float currentTime, float noteSpeed, float position, int index)
        {
            var highSpeed = _normalizedSpeed * noteSpeed;//NoteSpeedCalculatorと同じ

            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - judgeTime;
            var currentPos = PositionCalculator(t, highSpeed);
            var change = RhythmGamePresenter.checkSpeedChangeEntity && index != 0
                ? currentPos - PositionCalculator(RhythmGamePresenter.CalculatePassedTime(index) - judgeTime,
                    highSpeed)
                : currentPos - PositionCalculator(-judgeTime, highSpeed);
            
            return position + change;
        }

        public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}
