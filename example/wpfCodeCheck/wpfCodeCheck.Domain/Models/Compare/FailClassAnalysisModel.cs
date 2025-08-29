using wpfCodeCheck.Domain.Models.Files;

namespace wpfCodeCheck.Domain.Models.Compare
{
    public record FailClassAnalysisModel
    {
        public FileEntityModel InputFile { get; init; } = new FileEntityModel();
        public FileEntityModel OutputFile { get; init; } = new FileEntityModel();
        public bool IsSelected { get; set; } = false;
    }
}