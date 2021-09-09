#nullable enable

using Rhythmium;

namespace Reilas
{
    public sealed class ReilasChartEntity : ChartEntity<ReilasNoteEntity, ReilasNoteLineEntity>
    {
        public ReilasChartEntity Mirror()
        {
            return base.Mirror<ReilasChartEntity>();
        }
    }
}
