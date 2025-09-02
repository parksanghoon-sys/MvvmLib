using wpfCodeCheck.Domain.Enums;
using CoreMvvmLib.Design.Enums;

namespace wpfCodeCheck.Domain.Models
{
    public class CompareEntity
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Checksum { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int LineCount { get; set; }
        public string CreateDate { get; set; }
        public EFileType FileType { get; set; } 
        public CompareStatus Status { get; set; } = CompareStatus.Same;
        public bool IsComparison { get; set; }
        
        // Input/Output 파일 관련 속성
        public string InputFileName { get; set; } = string.Empty;
        public string InputFilePath { get; set; } = string.Empty;
        public string OutoutFileName { get; set; } = string.Empty;
        public string OutoutFilePath { get; set; } = string.Empty;
        
        public CompareEntity? CompareTarget { get; set; }
        
        // 생성자
        public CompareEntity() { }
        
        public CompareEntity(FileTreeModel fileModel)
        {
            FilePath = fileModel.FilePath;
            FileName = fileModel.FileName;
            Checksum = fileModel.Checksum;
            FileSize = fileModel.FileSize ?? 0;
            LineCount = fileModel.LineCount;
            CreateDate = fileModel.CreateDate;
            FileType = fileModel.FileType;
            Status = fileModel.Status;
            IsComparison = fileModel.IsComparison;
        }
        public override string ToString()
        {
            return $"Input : {InputFileName}; Output : {OutoutFileName} Status : {Status.ToString()}" ;
        }
    }
}