#nullable enable

using Rhythmium;
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

        public static float gameSpeed;

        public static void CalculateGameSpeed()
        {
            gameSpeed = PlayerPrefs.HasKey("rate") ? 10 * PlayerPrefs.GetFloat("rate") : 10;
        }
        
        public static Vector3 GetPosition(NoteEntity entity, float currentTime, bool checkIfTap, float noteSpeed)
        {
            float highSpeed = gameSpeed * noteSpeed;

            var size = entity.Size * BelowNoteWidth;
            var left = size / 2f;
            var pos = LeftPosition + (left);

            Vector3 notePos;

            pos += entity.LanePosition * BelowNoteWidth;

            //var toLeft = left - LeftPosition;

            var x = pos;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - entity.JudgeTime;
            var normalizedTime = -t * highSpeed / 600f;
            
            if (checkIfTap)
            {
                if (normalizedTime < 0 || normalizedTime >= 1)
                {
                    notePos = new Vector3(0f, 0f, 999f);
                }
                else
                {
                    notePos = new Vector3(x, 0f, highSpeed / 2 * t * t - highSpeed * t);
                }
            }
            else
            {
                notePos = normalizedTime < 0 ? new Vector3(x, 0f, -highSpeed * t) : new Vector3(x, 0f, highSpeed / 2 * t * t - highSpeed * t);
            }

            return notePos;
        }

        public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}