using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Datas
{
    public class FileCompareModel : FileItem, IEquatable<FileItem>
    {        
        public bool IsComparison { get; set; }   

        public int FileIndex { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int Depth { get; set; }
        public EFileType FileType { get; set; }
        public virtual List<FileCompareModel>? Children { get; set; }

        public bool Equals(FileItem? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return (this.Checksum == other.Checksum && this.FileName == other.FileName && this.CreateDate == other.CreateDate);
        }
        public override string ToString()
        {
            return $"{IsComparison}|{FileName} | {Checksum} | {LineCount}";
        }     
    }
    public static class FileCompareModelExtensions
    {
        // 재귀적으로 모든 노드(자식 포함)를 열거하는 메서드
        public static IEnumerable<FileCompareModel> Flatten(this IEnumerable<FileCompareModel> source)
        {
            foreach (var item in source)
            {
                yield return item;

                if (item.Children != null && item.Children.Any())
                {
                    foreach (var child in item.Children.Flatten())
                    {
                        yield return child;
                    }
                }
            }
        }
        public static List<FileCompareModel> FilterSourceCodeTree(this IEnumerable<FileCompareModel> items)
        {
            return items
                .Select(item =>
                {
                    // 자식들 먼저 필터링
                    var filteredChildren = item.Children?.FilterSourceCodeTree();

                    // 현재 노드가 SOURCECODE 이거나 자식에 SOURCECODE 가 있으면 포함
                    if (item.FileType == EFileType.SOURCECODE || (filteredChildren != null && filteredChildren.Any()))
                    {
                        // 깊은 복사: 원본을 수정하지 않고 새로운 객체 생성
                        var filteredItem = new FileCompareModel
                        {
                            IsComparison = item.IsComparison,
                            FileIndex = item.FileIndex,
                            ProjectName = item.ProjectName,
                            Depth = item.Depth,
                            FileType = item.FileType,
                            FileName = item.FileName,
                            FilePath = item.FilePath,
                            FileSize = item.FileSize,
                            Checksum = item.Checksum,
                            LineCount = item.LineCount,
                            CreateDate = item.CreateDate,
                            Children = filteredChildren
                        };
                        return filteredItem;
                    }

                    // 그렇지 않으면 제외
                    return null;
                })
                .Where(x => x != null)
                .ToList()!;
        }
    }
}
