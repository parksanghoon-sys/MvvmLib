using CompareEngine;
using System.Xml.Linq;
using YamlDotNet.Core.Tokens;

namespace wpfCodeCheck.Shared.Local.Models
{
    public record CodeCompareModel 
    {
        private IList<CompareResult>? _compareResults = new List<CompareResult>();

        public IList<CompareResult> CompareResults
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
        public string FileName{ get; set; }
    }
}
