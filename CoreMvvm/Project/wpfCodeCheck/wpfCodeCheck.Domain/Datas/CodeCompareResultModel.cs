using CompareEngine;

namespace wpfCodeCheck.Domain.Datas
{
    using ProjectName = string;
    public record CodeCompareResultModel
    {
        private IDictionary<ProjectName, IList<CompareResult>>? _compareResults = new Dictionary<ProjectName, IList<CompareResult>>();

        public IDictionary<ProjectName, IList<CompareResult>> CompareResults
        {
            get => _compareResults ?? throw new ArgumentNullException(nameof(CompareResults), "CompareResults cannot be null");
            set => _compareResults = value ?? throw new ArgumentNullException(nameof(value), "CompareResults cannot be null");
        }
    }
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
        public string FileName { get; set; } = string.Empty;
    }
}
