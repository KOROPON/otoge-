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
        private const float BelowNoteWidth = 2.5f;
        private const float LeftPosition = -5f;

        public static Vector3 GetPosition(NoteEntity entity, float currentTime)
        {
            const float highSpeed = 300f;


            var size = entity.Size * BelowNoteWidth;

            var left = size / 2f;

            var pos = LeftPosition + (left);

            pos += entity.LanePosition * BelowNoteWidth;

            var toLeft = left - LeftPosition;

            var x = pos; //left - LeftPosition;

            // var x = 3.75f + entity.LanePosition * BelowNoteWidth;

            return new Vector3(x, 0f, (currentTime - entity.JudgeTime) * highSpeed);
        }

        public static Vector3 GetScale(NoteEntity entity, float y = 1f)
        {
            return new Vector3(entity.Size * BelowNoteWidth, y, 1f);
        }
    }
}