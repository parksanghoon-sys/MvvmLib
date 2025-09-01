using System.IO;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting
{
    /// <summary>
    /// 표준 파일 트리 정렬기
    /// 우선순위: 1) 실행파일(.exe), 2) 라이브러리(.dll), 3) 디렉토리, 4) 기타 파일
    /// </summary>
    public class StandardFileTreeSorter : IFileTreeSorter
    {
        public string SorterName => "Standard";

        public List<FileTreeModel> SortFileTree(List<FileTreeModel> files)
        {
            if (files == null || !files.Any())
                return files ?? new List<FileTreeModel>();

            return SortFilesRecursively(files);
        }

        private List<FileTreeModel> SortFilesRecursively(List<FileTreeModel> files)
        {
            var sorted = files
                .OrderBy(GetFilePriority)
                .ThenBy(file => file.FileName, StringComparer.OrdinalIgnoreCase)
                .Select(file =>
                {
                    // 자식 파일들도 재귀적으로 정렬
                    if (file.Children?.Any() == true)
                    {
                        file.Children = SortFilesRecursively(file.Children);
                    }
                    return file;
                })
                .ToList();

            return sorted;
        }

        /// <summary>
        /// 파일 타입별 우선순위 결정
        /// </summary>
        private int GetFilePriority(FileTreeModel file)
        {
            if (file.IsDirectory)
                return 2; // 디렉토리는 중간 우선순위

            var extension = Path.GetExtension(file.FileName).ToLower();
            
            return extension switch
            {
                ".exe" => 0,    // 실행파일 최우선
                ".dll" => 1,    // 라이브러리 두 번째
                _ => 3          // 나머지 파일들
            };
        }
    }
}