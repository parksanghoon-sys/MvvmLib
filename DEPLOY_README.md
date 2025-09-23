# CoreMvvmLib 누겟 배포 가이드

## 📦 통합된 패키지 구조

이제 CoreMvvmLib는 **2개의 통합 패키지**로 제공됩니다:

### 1. CoreMvvmLib (핵심 패키지)
- ✅ **CoreMvvmLib.Core.Common** - 기본 MVVM 클래스들
- ✅ **CoreMvvmLib.Core.IOC** - 의존성 주입 컨테이너
- ✅ **CoreMvvmLib.Core.Services** - 핵심 서비스들
- ✅ **CoreMvvmLib.SourceGeneration** - 코드 생성기

### 2. CoreMvvmLib.WPF (WPF 통합 패키지)
- ✅ **CoreMvvmLib.Design** - 디자인 도구들
- ✅ **CoreMvvmLib.Component** - UI 컴포넌트들
- ✅ WPF 관련 모든 기능

## 🚀 배포 스크립트 사용법

### 1. 버전 관리

```powershell
# 현재 버전 확인
./update-version.ps1 -ShowCurrent

# 패치 버전 증가 (1.0.0 → 1.0.1)
./update-version.ps1 -BumpType patch

# 마이너 버전 증가 (1.0.1 → 1.1.0)
./update-version.ps1 -BumpType minor

# 메이저 버전 증가 (1.1.0 → 2.0.0)
./update-version.ps1 -BumpType major

# 프리릴리즈 버전
./update-version.ps1 -BumpType patch -Suffix "beta"
```

### 2. 패키지 빌드 및 배포

```powershell
# 패키지만 빌드 (배포하지 않음)
./publish-nuget.ps1 -PackOnly

# 버전 증가하면서 배포
./publish-nuget.ps1 -BumpVersion patch

# 확인 없이 배포
./publish-nuget.ps1 -SkipConfirm

# Release 빌드로 배포 (기본값)
./publish-nuget.ps1 -Configuration Release
```

### 3. 전체 워크플로우 예시

```powershell
# 1. 버전 업데이트
./update-version.ps1 -BumpType patch

# 2. 패키지 빌드 및 배포
./publish-nuget.ps1 -SkipConfirm
```

## 📁 파일 구조

```
MvvmLib/
├── .env                    # 누겟 API 키 및 설정
├── Version.props           # 중앙 버전 관리
├── update-version.ps1      # 버전 관리 스크립트
├── publish-nuget.ps1       # 통합 배포 스크립트
└── artifacts/
    └── packages/           # 생성된 누겟 패키지들
```

## 🔧 환경 설정

### .env 파일 설정
```bash
NUGET_API_KEY=your_api_key_here
NUGET_SOURCE=https://api.nuget.org/v3/index.json
PROJECTS_TO_PACK=src/CoreMvvmLib;src/CoreMvvmLib.WPF
BUILD_CONFIGURATION=Release
AUTO_INCREMENT_VERSION=true
VERSION_BUMP_TYPE=patch
```

### Version.props 파일
```xml
<Project>
  <PropertyGroup>
    <VersionMajor>1</VersionMajor>
    <VersionMinor>1</VersionMinor>
    <VersionPatch>0</VersionPatch>
    <VersionSuffix></VersionSuffix>
    <!-- 기타 패키지 메타데이터 -->
  </PropertyGroup>
</Project>
```

## 📦 사용자 설치 방법

### 패키지 매니저 콘솔
```powershell
# 핵심 MVVM 기능만 필요한 경우
Install-Package CoreMvvmLib

# WPF UI 컴포넌트까지 필요한 경우
Install-Package CoreMvvmLib
Install-Package CoreMvvmLib.WPF
```

### PackageReference
```xml
<PackageReference Include="CoreMvvmLib" Version="1.1.0" />
<PackageReference Include="CoreMvvmLib.WPF" Version="1.1.0" />
```

## ✨ 장점

1. **간편한 설치**: 2개 패키지만 설치하면 모든 기능 사용 가능
2. **통합 버전 관리**: 모든 패키지가 동일한 버전으로 관리됨
3. **자동화된 배포**: 스크립트 하나로 빌드/배포 완료
4. **의존성 관리**: 필요한 DLL들이 자동으로 포함됨

## 🔍 트러블슈팅

### 배포 실패 시
- 버전이 이미 존재하는지 확인
- API 키가 올바른지 확인
- 모든 의존성 프로젝트가 빌드되었는지 확인

### 버전 관리 문제
- Version.props 파일 형식 확인
- Git 태그와 버전 일치 여부 확인

## 📝 변경 사항 로그

### v1.1.0
- 패키지 구조 통합 (8개 → 2개 패키지)
- 중앙 버전 관리 도입
- 자동화된 배포 스크립트 추가