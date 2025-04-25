using CompareEngine;

namespace wpfCodeCheck.Domain.Datas
{
    public record CustomCodeComparer : CompareEntity
    {
        public CustomCodeComparer()
        {
            CompareResultSpans = new List<CompareResultSpan>();
            InputCompareText = new();
            OutputCompareText = new();
        }
        public IList<CompareResultSpan> CompareResultSpans { get; set; }
        public CompareText InputCompareText { get; set; }
        public CompareText OutputCompareText { get; set; }

    }
}
