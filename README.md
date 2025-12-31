# MvvmLib

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET%20Core-3.1%2B-blue)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/parksanghoon-sys/MvvmLib/ci.yml?branch=main)](https://github.com/parksanghoon-sys/MvvmLib/actions)

**.NET Core 기반 WPF MVVM 및 IoC 제공 라이브러리**
 
MvvmLib은 WPF 애플리케이션 개발을 위한 포괄적인 MVVM(Model-View-ViewModel) 프레임워크입니다. 의존성 주입(IoC), 메시징 시스템, 모듈화 아키텍처를 통해 확장 가능하고 유지보수하기 쉬운 애플리케이션을 구축할 수 있습니다.

## 🚀 주요 기능

### 💡 핵심 MVVM 구성 요소

- **ViewModelBase**: INotifyPropertyChanged 구현이 포함된 기본 ViewModel 클래스
- **RelayCommand**: 매개변수가 있는/없는 명령 구현
- **ObservableCollection 확장**: 향상된 컬렉션 변경 알림

### 🔧 IoC (Inversion of Control)

- **의존성 주입 컨테이너**: 서비스 등록 및 해결
- **서비스 Locator 패턴**: 외부에서 안전한 Get Service 기능
- **생명주기 관리**: Singleton, Transient, Scoped 지원

### 📡 메시징 시스템

- **ViewModel 간 통신**: 느슨한 결합을 통한 데이터 전송
- **이벤트 기반 아키텍처**: 비동기 메시지 처리
- **타입 안전성**: 강타입 메시지 지원

### 🏗️ Region 기능

- **UI 영역 관리**: 동적 콘텐츠 로딩
- **Navigation 지원**: 지역별 탐색 관리
- **모듈 통합**: Region 기반 모듈 배치

### 📦 모듈 시스템

- **모듈화 아키텍처**: 플러그인 방식의 확장
- **동적 로딩**: 런타임 모듈 로드/언로드
- **독립적 개발**: 모듈별 독립 개발 지원

### 🛠️ 유틸리티 기능

- **파일 체크섬 계산**: 파일 무결성 검증
- **파일 비교 도구**: 두 파일 간 차이점 검출
- **테스트 지원**: 단위 테스트 및 통합 테스트 도구

## 📦 설치

### NuGet Package Manager

```
Install-Package MvvmLib
```

### .NET CLI

```bash
dotnet add package MvvmLib
```

### PackageReference

```xml
<PackageReference Include="MvvmLib" Version="1.0.0" />
```

## 🔧 빠른 시작

### 1. 기본 ViewModel 구현

```csharp
using MvvmLib.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private int _count;
    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }
}
```

### 2. Command 구현

```csharp
using MvvmLib.Commands;

public class MainViewModel : ViewModelBase
{
    public RelayCommand IncrementCommand { get; }
    public RelayCommand<string> ProcessCommand { get; }

    public MainViewModel()
    {
        IncrementCommand = new RelayCommand(IncrementCount, CanIncrement);
        ProcessCommand = new RelayCommand<string>(ProcessData);
    }

    private void IncrementCount()
    {
        Count++;
    }

    private bool CanIncrement()
    {
        return Count < 100;
    }

    private void ProcessData(string data)
    {
        // 데이터 처리 로직
    }
}
```

### 3. IoC 컨테이너 설정

```csharp
using MvvmLib.IoC;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // 서비스 등록
        ServiceContainer.Instance.RegisterSingleton<IDataService, DataService>();
        ServiceContainer.Instance.RegisterTransient<MainViewModel>();
        ServiceContainer.Instance.RegisterScoped<IUserService, UserService>();
      
        // 메인 윈도우 생성
        var mainWindow = new MainWindow
        {
            DataContext = ServiceContainer.Instance.GetService<MainViewModel>()
        };
      
        mainWindow.Show();
        base.OnStartup(e);
    }
}
```

### 4. 메시징 시스템 사용

```csharp
using MvvmLib.Messaging;

// 메시지 정의
public class UserSelectedMessage : MessageBase
{
    public User SelectedUser { get; set; }
}

// 메시지 발송
public class UserListViewModel : ViewModelBase
{
    private void OnUserSelected(User user)
    {
        MessengerInstance.Send(new UserSelectedMessage { SelectedUser = user });
    }
}

// 메시지 수신
public class UserDetailViewModel : ViewModelBase
{
    public UserDetailViewModel()
    {
        MessengerInstance.Register<UserSelectedMessage>(this, OnUserSelected);
    }

    private void OnUserSelected(UserSelectedMessage message)
    {
        CurrentUser = message.SelectedUser;
    }

    public override void Cleanup()
    {
        MessengerInstance.Unregister(this);
        base.Cleanup();
    }
}
```

### 5. Region 기능 활용

```csharp
using MvvmLib.Regions;

// XAML에서 Region 정의
// <ContentControl regions:RegionManager.RegionName="MainRegion" />

// 코드에서 Region 사용
public class ShellViewModel : ViewModelBase
{
    private readonly IRegionManager _regionManager;

    public ShellViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        NavigateCommand = new RelayCommand<string>(Navigate);
    }

    public RelayCommand<string> NavigateCommand { get; }

    private void Navigate(string viewName)
    {
        _regionManager.RequestNavigate("MainRegion", viewName);
    }
}
```

### 6. 모듈 시스템

```csharp
using MvvmLib.Modularity;

// 모듈 정의
public class UserModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion("UserRegion", typeof(UserListView));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<UserListView, UserListViewModel>();
        containerRegistry.RegisterForNavigation<UserDetailView, UserDetailViewModel>();
    }
}

// 모듈 로드
public partial class App : Application
{
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<UserModule>();
    }
}
```

## 🛠️ 유틸리티 기능

### 파일 체크섬 계산

```csharp
using MvvmLib.Utilities;

string filePath = @"C:\path\to\file.txt";
string checksum = FileUtility.CalculateChecksum(filePath);
Console.WriteLine($"파일 체크섬: {checksum}");
```

### 파일 비교

```csharp
using MvvmLib.Utilities;

string file1 = @"C:\path\to\file1.txt";
string file2 = @"C:\path\to\file2.txt";

var differences = FileUtility.CompareFiles(file1, file2);
foreach (var diff in differences)
{
    Console.WriteLine($"차이점: {diff}");
}
```

## 📚 API 문서

### ViewModelBase

```csharp
public abstract class ViewModelBase : INotifyPropertyChanged, ICleanup
{
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null);
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null);
    public virtual void Cleanup();
}
```

### RelayCommand

```csharp
public class RelayCommand : ICommand
{
    public RelayCommand(Action execute, Func<bool> canExecute = null);
    public event EventHandler CanExecuteChanged;
    public bool CanExecute(object parameter);
    public void Execute(object parameter);
    public void RaiseCanExecuteChanged();
}

public class RelayCommand<T> : ICommand
{
    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null);
}
```

### MessengerInstance

```csharp
public static class MessengerInstance
{
    public static void Register<T>(object recipient, Action<T> action) where T : MessageBase;
    public static void Send<T>(T message) where T : MessageBase;
    public static void Unregister(object recipient);
}
```

## 🎯 지원 플랫폼

- **.NET Core 3.1+**
- **.NET 5.0+**
- **.NET 6.0+**
- **WPF**

## 📋 시스템 요구사항

- **운영체제**: Windows 7 SP1 이상
- **프레임워크**: .NET Core 3.1 이상
- **개발환경**: Visual Studio 2019 이상 또는 Visual Studio Code

## 🚀 프로젝트 구조

```
MvvmLib/
├── src/
│   ├── MvvmLib/                 # 메인 라이브러리
│   │   ├── ViewModels/          # ViewModel 기본 클래스
│   │   ├── Commands/            # Command 구현체
│   │   ├── IoC/                 # IoC 컨테이너
│   │   ├── Messaging/           # 메시징 시스템
│   │   ├── Regions/             # Region 관리
│   │   ├── Modularity/          # 모듈 시스템
│   │   └── Utilities/           # 유틸리티 기능
│   └── MvvmLib.Extensions/      # 확장 기능
├── tests/
│   ├── MvvmLib.Tests/           # 단위 테스트
│   └── MvvmLib.IntegrationTests/ # 통합 테스트
├── samples/
│   ├── BasicSample/             # 기본 사용 예제
│   ├── ModularSample/           # 모듈 시스템 예제
│   └── AdvancedSample/          # 고급 기능 예제
└── docs/                        # 문서
```

## 🤝 기여하기

1. 프로젝트를 Fork합니다
2. 새로운 기능 브랜치를 생성합니다 (`git checkout -b feature/amazing-feature`)
3. 변경사항을 커밋합니다 (`git commit -m 'Add some amazing feature'`)
4. 브랜치에 Push합니다 (`git push origin feature/amazing-feature`)
5. Pull Request를 열어주세요

### 개발 환경 설정

```bash
# 저장소 클론
git clone https://github.com/parksanghoon-sys/MvvmLib.git
cd MvvmLib

# 의존성 복원
dotnet restore

# 빌드
dotnet build

# 테스트 실행
dotnet test
```

### 코딩 컨벤션

- **네이밍**: PascalCase (클래스, 메서드), camelCase (필드, 변수)
- **들여쓰기**: 4개 공백
- **주석**: XML 문서 주석 사용
- **테스트**: 모든 공개 API에 대한 단위 테스트 필수

## 📝 변경 로그

### [1.0.0] - 2024-06-24

- ✨ 초기 릴리스
- ✅ ViewModelBase 구현
- ✅ RelayCommand 구현
- ✅ IoC 컨테이너 추가
- ✅ 메시징 시스템 구현
- ✅ Region 기능 추가
- ✅ 모듈 시스템 구현
- ✅ 파일 유틸리티 기능 추가

## 📄 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

## 🙏 감사의 말

이 프로젝트는 다음 오픈소스 프로젝트들에서 영감을 받았습니다:

- [MVVM Light Toolkit](https://github.com/lbugnion/mvvmlight) by Laurent Bugnion
- [Prism](https://github.com/PrismLibrary/Prism) by PrismLibrary
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) by Microsoft

## 📖 관련 자료

- [WPF MVVM 패턴 가이드](https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/advanced/model-view-viewmodel)
- [의존성 주입 패턴](https://learn.microsoft.com/ko-kr/dotnet/core/extensions/dependency-injection)
- [.NET Core WPF 개발](https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/)

---

⭐ **이 프로젝트가 유용하다면 스타를 눌러주세요!**

Made with ❤️ by [parksanghoon-sys](https://github.com/parksanghoon-sys)
