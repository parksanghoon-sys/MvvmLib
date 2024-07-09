using CompareEngine;

namespace wpfCodeCheck.ConfigurationChange.Local.Services
{
    public record CompareResult
    {
        public CompareResult()
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
