using CompareEngine;
using System.Collections;

namespace onpenxmlTest
{
    public record CompareResult
    {
        public CompareResult()
        {
            CompareResultSpans = new List<CompareResultSpan>();
            InputCompareText = new();
            OutputCompareText = new();
        }
        public IList<CompareResultSpan> CompareResultSpans { get; set; }
        public CompareText InputCompareText { get; set; }
        public CompareText OutputCompareText { get; set; }
    }
    public static class CLIComparer
    {

        public static string LeftSide = "";
        public static string RightSide = "";


        public static ArrayList DiffLines;
        public static void Init()
        {
            DiffLines = new ArrayList();
        }
        public static bool GetFiles(string[] args)
        {
            //Get Arguments
            if (args != null)
            {
                foreach (string s in args)
                {
                    if (String.IsNullOrEmpty(LeftSide))
                    {
                        LeftSide = s;
                    }
                    else if (String.IsNullOrEmpty(RightSide))
                    {
                        RightSide = s;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //If I did not pass files in arguments then ask user for files
            if (String.IsNullOrEmpty(LeftSide) || String.IsNullOrEmpty(RightSide))
            {
                Console.WriteLine("Drag and Drop Path to File 1 (then press enter):\n");
                LeftSide = Console.ReadLine();

                Console.WriteLine("\nDrag and Drop Path to File 2 (then press enter):\n");
                RightSide = Console.ReadLine();
            }


            //Check if I have 2 valid files to compare
            if (!FileValidator.ValidFile(LeftSide))
            {
                CLIComparer.InvalidFile(LeftSide);
                return false;
            }
            if (!FileValidator.ValidFile(RightSide))
            {
                CLIComparer.InvalidFile(RightSide);
                return false;
            }

            return true;
        }
        public static void InvalidFile(string fileName)
        {
            Console.WriteLine("Invalid File Passed: " + fileName);
        }

        public static CompareResult GetCompareFiles()
        {
            CompareEngine.CompareEngine compareEngine = new CompareEngine.CompareEngine();

            //Load the file paths into objects
            CompareText sourceDiffList = new CompareText(LeftSide);
            CompareText destinationDiffList = new CompareText(RightSide);
            //Perform the comparison
            compareEngine.StartDiff(sourceDiffList, destinationDiffList);
            //Get Results
            ArrayList resultLines = compareEngine.DiffResult();

            CompareResult compareResult = new CompareResult();
            compareResult.InputCompareText = sourceDiffList;
            compareResult.OutputCompareText = destinationDiffList;
            compareResult.CompareResultSpans = GetArrayListToList< CompareResultSpan >(resultLines);

            return compareResult;
        }
        private static IList<T> GetArrayListToList<T>(ArrayList list)
        {
            List<T> result = new List<T>();

            foreach (var item in list)
            {
                if(item is T)
                {
                    result.Add((T)item);
                }
            }

            return result;
        }
        public static void CompareFiles()
        {
            CLIComparer.Init();

            //If I am here I have valid files, start comparing
            CompareEngine.CompareEngine compareEngine = new CompareEngine.CompareEngine();

            //Load the file paths into objects
            CompareText sourceDiffList = new CompareText(LeftSide);
            CompareText destinationDiffList = new CompareText(RightSide);
            //Perform the comparison
            compareEngine.StartDiff(sourceDiffList, destinationDiffList);
            //Get Results
            ArrayList resultLines = compareEngine.DiffResult();
            string numberMask = GetNumberMask(sourceDiffList.Count(), destinationDiffList.Count());

            int lineCounter = 1;

            foreach (CompareResultSpan compareResultSpan in resultLines)
            {
                switch (compareResultSpan.Status)
                {
                    case CompareResultSpanStatus.DeleteSource:
                        for (int i = 0; i < compareResultSpan.Length; i++)
                        {
                            string initial = "";
                            string rewrite = "";

                            initial += lineCounter.ToString(numberMask);
                            rewrite += lineCounter.ToString(numberMask);

                            initial += " < " + sourceDiffList.GetByIndex(compareResultSpan.SourceIndex + i).Line + "";
                            initial += "Source Line : " + (compareResultSpan.SourceIndex + i + 1).ToString();
                            CLIComparer.DiffLines.Add(initial);

                            lineCounter++;
                        }

                        break;
                    case CompareResultSpanStatus.NoChange:
                        for (int i = 0; i < compareResultSpan.Length; i++)
                        {
                            lineCounter++;
                        }

                        break;
                    case CompareResultSpanStatus.AddDestination:
                        for (int i = 0; i < compareResultSpan.Length; i++)
                        {

                            string rewrite = "";

                            rewrite += lineCounter.ToString(numberMask);


                            rewrite += " > " + destinationDiffList.GetByIndex(compareResultSpan.DestinationIndex + i).Line + "";
                            rewrite += "Source Line : " + (compareResultSpan.DestinationIndex + i + 1).ToString();
                            CLIComparer.DiffLines.Add(rewrite);

                            lineCounter++;
                        }

                        break;
                    case CompareResultSpanStatus.Replace:
                        for (int i = 0; i < compareResultSpan.Length; i++)
                        {
                            string initial = "";
                            string rewrite = "";

                            initial += lineCounter.ToString(numberMask);
                            rewrite += lineCounter.ToString(numberMask);

                            initial += " initial:";
                            rewrite += " rewrite:";

                            initial += sourceDiffList.GetByIndex(compareResultSpan.SourceIndex + i).Line;
                            rewrite += destinationDiffList.GetByIndex(compareResultSpan.DestinationIndex + i).Line;


                            initial += "Source Line : " + (compareResultSpan.SourceIndex + i + 1).ToString();
                            rewrite += "Source Line : " + (compareResultSpan.DestinationIndex + i + 1).ToString();

                            CLIComparer.DiffLines.Add(initial);
                            CLIComparer.DiffLines.Add(rewrite);

                            lineCounter++;
                        }

                        break;
                }
            }

        }

        private static string GetNumberMask(int sourceDiffListCount, int destinationDiffListCount)
        {
            string numberMask = "000";

            if (sourceDiffListCount > 999 && sourceDiffListCount > destinationDiffListCount)
            {
                for (int x = 0; x < sourceDiffListCount.ToString().Length; x++)
                {
                    numberMask += "0";
                }
            }
            else if (destinationDiffListCount > 999)
            {
                for (int x = 0; x < destinationDiffListCount.ToString().Length; x++)
                {
                    numberMask += "0";
                }
            }

            return numberMask;
        }
    }
}
