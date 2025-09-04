using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.Domain.Services
{
    /// <summary>
    /// 애플리케이션의 기본 서비스 기능을 제공하는 클래스
    /// 폴더 타입별 파일 데이터와 비교 결과를 관리합니다.
    /// </summary>
    public class BaseService : BaseModel, IBaseService
    {        
        /// <summary>
        /// 폴더 타입별 파일 트리 모델을 저장하는 딕셔너리
        /// </summary>
        private Dictionary<EFolderListType, List<FileTreeModel>> _folderTypeDictionaryFiles = new();
        /// <summary>
        /// 디렉토리 비교 결과 목록
        /// </summary>
        private List<CompareEntity> _compareResult = new();        
        /// <summary>
        /// BaseService 생성자
        /// 입력 및 출력 폴더 타입에 대한 빈 리스트를 초기화합니다.
        /// </summary>
        public BaseService()
        {
            _folderTypeDictionaryFiles[EFolderListType.INPUT] = new List<FileTreeModel>();
            _folderTypeDictionaryFiles[EFolderListType.OUTPUT] = new List<FileTreeModel>();
        }

        /// <summary>
        /// 지정된 폴더 타입에 대한 파일 리스트를 설정합니다.
        /// </summary>
        /// <param name="folderType">폴더 타입</param>
        /// <param name="files">설정할 파일 목록</param>
        public void SetFolderTypeDictionaryFiles(EFolderListType folderType, List<FileTreeModel> files)
        {
            _folderTypeDictionaryFiles[folderType] = files;
            OnPropertyChanged(nameof(FolderTypeDictionaryFiles));
        }

        /// <summary>
        /// 폴더 타입별 파일 리스트를 가져옵니다.
        /// </summary>
        public Dictionary<EFolderListType, List<FileTreeModel>> FolderTypeDictionaryFiles 
            => _folderTypeDictionaryFiles;

        /// <summary>
        /// 디렉토리 비교 결과를 설정합니다.
        /// </summary>
        /// <param name="compareResult">비교 결과 목록</param>
        public void SetDirectoryCompareReuslt(List<CompareEntity> compareResult)
        {
            _compareResult = compareResult;
            OnPropertyChanged(nameof(DirectoryCompareResult));
        }

        /// <summary>
        /// 디렉토리 비교 결과를 가져옵니다.
        /// </summary>
        public List<CompareEntity> DirectoryCompareResult => _compareResult;
        
        /// <summary>
        /// 비교 결과를 가져옵니다. (레거시 호환성을 위한 별칭)
        /// </summary>
        public List<CompareEntity> CompareResult => _compareResult;

        /// <summary>
        /// 지정된 폴더 타입에 대한 파일 리스트를 가져옵니다.
        /// </summary>
        /// <param name="folderType">가져올 폴더 타입</param>
        /// <returns>해당 폴더 타입의 파일 목록, 없으면 빈 리스트</returns>
        public List<FileTreeModel> GetFolderTypeDictionaryFiles(EFolderListType folderType)
        {
            return _folderTypeDictionaryFiles.TryGetValue(folderType, out var files) ? files : new List<FileTreeModel>();
        }      
    }
}
