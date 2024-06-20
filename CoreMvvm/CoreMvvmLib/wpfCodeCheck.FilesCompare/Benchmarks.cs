using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;

namespace wpfCodeCheck.FilesCompare
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        [Benchmark]
        public void Scenario1()
        {
            string filePath1 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight";
            string filePath2 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21. PPC 소스파일\Source\FlightSolution";

            IDierctoryFileInfoService dierctoryFileInfo1 = new DirectoryService(new Crc32FileChecSum());
            IDierctoryFileInfoService dierctoryFileInfo2 = new DirectoryService(new Crc32FileChecSum());

            var fileList = dierctoryFileInfo1.GetDirectoryCodeFileInfos(filePath1);
            var secondsList = dierctoryFileInfo2.GetDirectoryCodeFileInfos(filePath2);


        }

    }
}
