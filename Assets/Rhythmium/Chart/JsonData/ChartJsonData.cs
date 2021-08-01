using System;

// ReSharper disable InconsistentNaming

namespace Rhythmium
{
    [Serializable]
    public class ChartJsonData
    {
        public int musicGameSystemVersion;
        public int difficulty;
        public string level;
        public string name;
        public string audioSource;
        public float startTime;
        public TimelineJsonData timeline;
    }
}
