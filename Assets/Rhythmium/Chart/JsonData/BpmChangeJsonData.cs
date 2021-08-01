using System;

// ReSharper disable InconsistentNaming

namespace Rhythmium
{
    [Serializable]
    public sealed class BpmChangeJsonData
    {
        public int measureIndex;
        public FractionJsonData measurePosition;
        public float bpm;
        public string guid;
    }
}
