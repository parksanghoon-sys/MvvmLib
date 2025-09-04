using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.Domain.Enums
{
    /// <summary>
    /// 비교 유형을 나타내는 열거형
    /// </summary>
    public enum ECompareType
    {
        /// <summary>
        /// 비교 없음
        /// </summary>
        NONE,
        /// <summary>
        /// 소스코드 비교
        /// </summary>
        SOURCECODE,
        /// <summary>
        /// 파일 비교
        /// </summary>
        FILE
    }
    
    /// <summary>
    /// 파일 유형을 나타내는 열거형
    /// </summary>
    public enum EFileType
    {
        /// <summary>
        /// 디렉토리
        /// </summary>
        DIRECTORY,
        /// <summary>
        /// 파일
        /// </summary>
        FILE
    }
    public enum EProjectType
    { 
        MUAV,
        B6
    }

}
