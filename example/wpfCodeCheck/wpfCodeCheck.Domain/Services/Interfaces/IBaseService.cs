using System.ComponentModel;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services.Interfaces;

/// <summary>
/// 애플리케이션의 기본 서비스 기능을 정의하는 인터페이스
/// 폴더 타입별 파일 데이터와 비교 결과 관리 기능을 제공합니다.
/// </summary>
public interface IBaseService : INotifyPropertyChanged
{
    /// <summary>
    /// 폴더 타입별 파일 목록을 가져옵니다.
    /// </summary>
    Dictionary<EFolderListType, List<FileTreeModel>> FolderTypeDictionaryFiles { get; }   
    /// <summary>
    /// 디렉토리 비교 결과를 가져옵니다.
    /// </summary>
    List<CompareEntity> DirectoryCompareResult { get; }
    
    /// <summary>
    /// 비교 결과를 가져옵니다. (레거시 호환성을 위한 별칭)
    /// </summary>
    List<CompareEntity> CompareResult { get; }

    /// <summary>
    /// 지정된 폴더 타입에 대한 파일 목록을 설정합니다.
    /// </summary>
    /// <param name="folderType">폴더 타입</param>
    /// <param name="files">설정할 파일 목록</param>
    void SetFolderTypeDictionaryFiles(EFolderListType folderType, List<FileTreeModel> files);
    /// <summary>
    /// 디렉토리 비교 결과를 설정합니다.
    /// </summary>
    /// <param name="compareResult">설정할 비교 결과 목록</param>
    void SetDirectoryCompareReuslt(List<CompareEntity> compareResult);
    /// <summary>
    /// 지정된 폴더 타입에 대한 파일 목록을 가져옵니다.
    /// </summary>
    /// <param name="folderType">가져올 폴더 타입</param>
    /// <returns>해당 폴더 타입의 파일 목록</returns>
    List<FileTreeModel> GetFolderTypeDictionaryFiles(EFolderListType folderType);
}
