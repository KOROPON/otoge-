using System;
using Guid = System.String;

// ReSharper disable InconsistentNaming

namespace Rhythmium
{
    [Serializable]
    public struct NoteCustomPropsJsonData
    {
        public string type;
        public Guid targetNoteLine;
        public float scale;
        public float distance;
    }

    [Serializable]
    public sealed class NoteJsonData
    {
        public Guid guid;
        public int horizontalSize;
        public FractionJsonData horizontalPosition;
        public int measureIndex;
        public FractionJsonData measurePosition;
        public string type;
        public float speed;
        public Guid lane;
        public NoteCustomPropsJsonData customProps;

        public override string ToString()
        {
            return
                $"{type} m({measureIndex + measurePosition.To01()}) h({horizontalPosition.denominator}/{horizontalPosition.denominator}) s({horizontalSize}) {guid}";
        }

        public NoteJsonData Mirror()
        {
            var mirroredNumerator =
                horizontalPosition.denominator - 1 - horizontalPosition.numerator - (horizontalSize - 1);
            var mirroredHorizontalPosition = new FractionJsonData
            {
                numerator = mirroredNumerator,
                denominator = horizontalPosition.denominator
            };

            return new NoteJsonData
            {
                guid = guid,
                horizontalSize = horizontalSize,
                horizontalPosition = mirroredHorizontalPosition,
                measureIndex = measureIndex,
                measurePosition = measurePosition,
                type = type,
                speed = speed,
                lane = lane,
                customProps = customProps
            };
        }
    }
}