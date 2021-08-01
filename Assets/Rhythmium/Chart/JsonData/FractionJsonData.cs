using System;
using Guid = System.String;

// ReSharper disable InconsistentNaming

namespace Rhythmium
{
    [Serializable]
    public sealed class FractionJsonData
    {
        public int numerator;
        public int denominator;

        public float To01()
        {
            return 1f / denominator * numerator;
        }
    }
}
