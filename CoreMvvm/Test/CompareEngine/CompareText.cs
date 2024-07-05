using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareEngine
{
    public class CompareText
    {
        private const int MaxLineLength = int.MaxValue / 2;
        private readonly ArrayList lines;

        public CompareText()
        {
            lines = new ArrayList();
        }
        public CompareText(string fileName)            
        {
            lines = new ArrayList();
            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if(line.Length > MaxLineLength)
                    {
                        throw new InvalidOperationException(
                            string.Format("File contains a line greater than {0} characters.",
                                          MaxLineLength));
                    }
                    lines.Add(new CompareTextLine(line));
                }
            }
            
        }

        public int Count()
        {
            return lines.Count;
        }

        public CompareTextLine GetByIndex(int index) => (CompareTextLine)lines[index];
    }
}
