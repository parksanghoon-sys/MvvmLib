using CompareEngine;

namespace wpfCodeCheck.Domain.Datas
{
    using ProjectName = string;
    public record CodeDiffModel
    {
        private IDictionary<ProjectName, IList<CodeComparer>>? _compareResults = new Dictionary<ProjectName, IList<CodeComparer>>();

        public IDictionary<ProjectName, IList<CodeComparer>> CompareResults
        {
            get => _compareResults ?? throw new ArgumentNullException(nameof(CodeDiffModel), "CompareResults cannot be null");
            set => _compareResults = value ?? throw new ArgumentNullException(nameof(value), "CompareResults cannot be null");
        }
    }
    public record CodeComparer
    {
        public CodeComparer()
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
