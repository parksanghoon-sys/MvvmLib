// 전역 using 설정으로 네임스페이스 사용 편의성 향상

global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// CoreMvvmLib 네임스페이스들


// .NET Standard 2.0 호환성
#if NETSTANDARD2_0
global using System.Collections.Concurrent;
global using System.Runtime.CompilerServices;
#endif