﻿using BenchmarkDotNet.Attributes;
using wpfCodeCheck.Main.Local.Servies;

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

            var dierctoryFileInfo1 = new SourceDirectoryService(new Crc32FileChecSum());
            var dierctoryFileInfo2 = new SourceDirectoryService(new Crc32FileChecSum());

            var fileList = dierctoryFileInfo1.GetDirectoryCodeFileInfosAsync(filePath1);
            var secondsList = dierctoryFileInfo2.GetDirectoryCodeFileInfosAsync(filePath2);

        }
        [Benchmark]
        public void Scenario2()
        {
            string filePath1 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight";
            string filePath2 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21_PPC 소스파일\Source\FlightSolution";

            var dierctoryFileInfo1 = new SourceDirectoryService(new Crc32FileChecSum());
            var dierctoryFileInfo2 = new SourceDirectoryService(new Crc32FileChecSum());

            var fileList = dierctoryFileInfo1.GetDirectoryCodeFileInfosAsync(filePath1);
            var secondsList = dierctoryFileInfo2.GetDirectoryCodeFileInfosAsync(filePath2);

        }
    }
}
