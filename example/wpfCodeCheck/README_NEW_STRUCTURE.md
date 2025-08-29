# 새로운 wpfCodeCheck 구조

## 주요 개선사항

### 1. 통합된 FileTreeModel
- 기존의 FileEntity, FileItem, FileCompareModel, CompareEntity 등 모든 모델을 하나로 통합
- 트리 구조 + 파일 정보 + 비교 상태를 모두 포함
- UI 바인딩 속성까지 통합 제공

### 2. 간소화된 서비스 구조
- **FileTreeService**: 디렉토리 검색과 파일 비교를 통합 처리
- **BaseService**: 3개 메서드로 모든 기능 제공
- **CodeCompareService**: CompareEngine 기반 상세 비교

### 3. 사용 방법

#### 기본 사용 패턴
```csharp
// BaseService 사용
var baseService = new BaseService();

// 1. 입력 디렉토리 로드
await baseService.LoadInputDirectoryAsync("C:/input");

// 2. 출력 디렉토리 로드  
await baseService.LoadOutputDirectoryAsync("C:/output");

// 3. 디렉토리 비교 실행
await baseService.CompareDirectoriesAsync();

// 4. UI 바인딩
// - InputFiles: 입력 디렉토리의 모든 파일
// - OutputFiles: 출력 디렉토리의 모든 파일  
// - DifferenceFiles: 차이가 있는 파일들만
```

#### FileTreeModel 주요 속성
```csharp
public class FileTreeModel
{
    // 파일 정보
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public long? FileSize { get; set; }
    public string Checksum { get; set; }
    
    // 트리 구조
    public List<FileTreeModel> Children { get; set; }
    public FileTreeModel? Parent { get; set; }
    public bool IsDirectory { get; }
    
    // 비교 상태
    public bool IsComparison { get; set; } // UI 바인딩용
    public FileTreeModel? CompareTarget { get; set; }
    public CompareStatus Status { get; set; }
    
    // UI 속성
    public bool IsExpanded { get; set; }
    public bool IsSelected { get; set; }
    public IconType IconType { get; }
}
```

#### 차이점 필터링
```csharp
// 차이가 있는 파일들만 가져오기
var differentFiles = fileTree.GetDifferentFiles().ToList();

// 모든 하위 파일들 평면화
var allFiles = fileTree.GetAllDescendants().Where(f => !f.IsDirectory);
```

### 4. 장점

1. **단순함**: 하나의 모델로 모든 것을 처리
2. **일관성**: 동일한 구조를 모든 곳에서 사용
3. **성능**: 중복 변환 작업 제거
4. **유지보수**: 모델 관련 버그 대폭 감소
5. **확장성**: 새로운 속성 추가가 쉬움

### 5. 마이그레이션 가이드

#### Before (기존)
```csharp
// 복잡한 여러 단계
var inputFiles = directoryService.GetFiles(inputPath);
var outputFiles = directoryService.GetFiles(outputPath);
var compareResult = compareService.Compare(inputFiles, outputFiles);
var diffResult = new DiffResultModel<CompareEntity>();
// ... 복잡한 변환 과정
```

#### After (새로운 구조)
```csharp
// 간단한 3단계
await baseService.LoadInputDirectoryAsync(inputPath);
await baseService.LoadOutputDirectoryAsync(outputPath);
await baseService.CompareDirectoriesAsync();
// UI에서 baseService.DifferenceFiles 바인딩
```