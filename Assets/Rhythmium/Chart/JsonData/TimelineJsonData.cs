using System;

// ReSharper disable InconsistentNaming

namespace Rhythmium
{
    [Serializable]
    public sealed class TimelineJsonData
    {
        public NoteJsonData[] notes;
        public NoteLineJsonData[] noteLines;
        public MeasureJsonData[] measures;
        public OtherObjectJsonData[] otherObjects;
    }
}
