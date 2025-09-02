using System.IO;
using System.Reflection;
using System.Text;
using wpfCodeCheck.Domain.Helpers;

namespace wpfCodeCheck.Domain.Local.Helpers
{
    public class CsvHelper : ICsvHelper
    {
        public CsvHelper()
        {
            
        }
        /// <summary>
        /// 컬렉션 CSV 파일 만들기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">컬렉션</param>
        /// <param name="path">파일 위치</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <param name="writeHeader">헤더 작성 여부</param>
        /// <returns>작업 완료 여부</returns>
        public bool CreateCSVFile<T>(ICollection<T> collection, string filename, bool overwrite = true, bool writeHeader = true)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (collection.Count > 0)
                {
                    var exportPath = DirectoryHelper.GetLocalExportDirectory();
                    DirectoryHelper.CreateDirectory(exportPath);
                    filename += ".csv";
                    var path = Path.Combine(exportPath, filename);

                    if (!File.Exists(path) || overwrite)
                    {
                        using (var writer = new FileStream(path, FileMode.Create, FileAccess.Write))
                        using (StreamWriter streamWriter = new StreamWriter(writer, Encoding.UTF8))
                        {
                            var enumerator = collection.GetEnumerator();
                            if (writeHeader)
                            {
                                // 1. 헤더 입력
                                if (enumerator.MoveNext())
                                {
                                    FieldInfo[] fieldInfos = GetAllFields(typeof(T));
                                    for (int i = 0; i < fieldInfos.Length; i++)
                                    {
                                        if (sb.Length > 0)
                                            sb.Append(',');
                                        sb.Append(fieldInfos[i].Name);
                                    }
                                    sb.Append('\n');
                                }
                            }

                            // 2. 데이터 입력
                            enumerator = collection.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                FieldInfo[] fieldInfos = GetAllFields(typeof(T));
                                for (int i = 0; i < fieldInfos.Length; i++)
                                {
                                    if (i > 0) sb.Append(',');
                                    sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                                }
                                sb.Append('\n');
                            }
                            sb.Length -= 1;

                            streamWriter.Write(sb.ToString());
                        }

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("이미 파일이 존재합니다.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        private FieldInfo[] GetAllFields(Type t)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            while (t != null)
            {
                fieldInfos.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
                t = t.BaseType;
            }
            return fieldInfos.ToArray();
        }
        /// <summary>
        /// 텍스트 파일 열기
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[,] OpenCSVFile(string path)
        {
            try
            {
                path += ".csv";
                if (File.Exists(path))
                {
                    string[] strs = File.ReadAllLines(path);
                    if (strs != null && strs.Length > 0)
                    {
                        int col = strs[0].Split(',').Length;
                        int row = strs.Length;

                        string[,] result = new string[row, col];
                        for (int i = 0; i < result.GetLength(0); i++)
                        {
                            string[] split = strs[i].Split(',');
                            for (int j = 0; j < result.GetLength(1); j++)
                            {
                                result[i, j] = split[j];
                            }
                        }
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
        /// <summary>
        /// 소프트웨어 변경사항 Excel Export시 Paste 동작 중
        /// System.Runtime.InteropServices.COMException: '0x800A03EC'  
        /// 오류 사항으로 인해 수작업으로 해야하는 경우에 Exception Log 기록 추출
        /// </summary>
        /// <param name="path"></param>
        /// <param name="colunms"></param>
        public void ExcepCOMExceptionToCsv(string path, params string[] colunms)
        {
            bool fileExists = File.Exists(path);
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))  // true: 이어쓰기 모드
                {
                    // 파일이 없었을 경우 헤더 작성
                    if (!fileExists)
                    {
                        sw.WriteLine("Timestamp,InputClass,OutputClass,InputFilePath,OutputFilePath");
                    }
                    // 예외 로그 작성
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{colunms[0]},{colunms[1]},{colunms[2]},{colunms[3]}");
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine("CSV 파일에 로그를 기록하는 중 오류가 발생했습니다: " + logEx.Message);
            }
        }
    }
}
