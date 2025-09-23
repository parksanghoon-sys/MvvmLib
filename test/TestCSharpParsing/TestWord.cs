
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;


namespace cliCSharpParsing
{
    public class TestWord
    {
        public double processPer = 0;
        Document word_doc;
        object missing = System.Reflection.Missing.Value;
        Microsoft.Office.Interop.Word.Application app_word;
        object EndOfDoc = "\\endofdoc";
        public void CreateWordFile(string path, List<ClassInfo> ClassCollection)
        {
            processPer = 0;
            app_word = new Microsoft.Office.Interop.Word.Application();
            app_word.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
            app_word.Visible = false;
            word_doc = app_word.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            double allCount = 0;
            try
            {
                Word.Section section;
                Word.Paragraph title = word_doc.Content.Paragraphs.Add();

                title.Range.Text = "SourceCheck 결과 파일";
                title.Range.Font.Size = 10;

                for (int i = 0; i < ClassCollection.Count; i++)
                {
                    allCount++;
                    foreach (var data in ClassCollection[i].Variables)
                    {
                        allCount++;
                    }
                }

                for (int i = 0; i < ClassCollection.Count; i++)
                {
                    CreateMainTable(i, ClassCollection[i], ClassCollection[i].FunctionInfos, ClassCollection[i].Variables);
                    processPer += 100.0 / allCount;
                    Console.WriteLine(processPer.ToString());
                    section = word_doc.Sections.Add();

                    foreach (var data in ClassCollection[i].FunctionInfos)
                    {
                        CreateFuncTable(ref section, ClassCollection[i].ClassPath, data);
                        processPer += 100.0 / allCount;
                    }
                }

                app_word.DisplayAlerts = Word.WdAlertLevel.wdAlertsAll;
                word_doc.SaveAs2(path);
                Dispose();
            }
            catch (Exception e)
            {
                //Dispose();
                Console.WriteLine(e.ToString());
            }
            finally
            {
                ReleaseObject(app_word);
                ReleaseObject(word_doc);
                GC.Collect();
            }
        }
        public void Dispose()
        {
            if (app_word != null)
            {
                app_word.Quit();
            }

            ReleaseObject(app_word);
            ReleaseObject(word_doc);
            GC.Collect();
        }

        void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
        private void CreateMainTable(int idx, ClassInfo classInfo, List<FunctionInfo> funcCollection, List<VariableInfo> valCollection)
        {
            Word.Section section1 = word_doc.Sections.Add();

            Word.Paragraph wParagrah = word_doc.Content.Paragraphs.Add();
            wParagrah.Range.Font.Size = 10;
            wParagrah.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            wParagrah.Range.Text = string.Format("경로 : {0} \n", classInfo.ClassPath);
            wParagrah.Range.InsertParagraphAfter();

            wParagrah = word_doc.Content.Paragraphs.Add();
            wParagrah.Range.Font.Size = 10;
            wParagrah.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            wParagrah.Range.Text = string.Format("{0}. {1} 클래스\n", idx + 1, classInfo.ClassName);

            Word.Table table;
            Word.Range tableRng = word_doc.Bookmarks.get_Item(ref EndOfDoc).Range;
            table = word_doc.Tables.Add(tableRng, 1, 4, ref missing, ref missing);
            table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineWidth = Word.WdLineWidth.wdLineWidth150pt;

            table.Cell(1, 1).Range.Text = "구분";
            table.Cell(1, 1).Range.Cells.Width = 30;

            table.Cell(1, 2).Range.Text = "형식(반환 값)";
            table.Cell(1, 2).Range.Cells.Width = 75;

            table.Cell(1, 3).Range.Text = "이  름";
            table.Cell(1, 3).Range.Cells.Width = 160;

            table.Cell(1, 4).Range.Text = "기  능";
            table.Cell(1, 4).Range.Cells.Width = 170;

            Word.Row row;

            for (int i = 0; i < valCollection.Count; i++)
            {
                row = table.Range.Rows.Add();

                table.Cell(i + 2, 2).Range.Text = valCollection[i].Type;
                table.Cell(i + 2, 3).Range.Text = valCollection[i].Name;
                table.Cell(i + 2, 4).Range.Text = valCollection[i].Summary;

                if (i == 0)
                {
                    table.Cell(2, 1).Range.Text = "변수";
                }
                else
                {
                    table.Columns[1].Cells[3].Merge(table.Columns[1].Cells[2]);
                }
            }

            int validx = valCollection.Count;

            int mergeIdx = validx > 0 ? 3 : 2;
            for (int i = 0; i < funcCollection.Count; i++)
            {
                row = table.Range.Rows.Add();

                table.Cell(i + 2 + validx, 2).Range.Text = funcCollection[i].ReturnType;
                table.Cell(i + 2 + validx, 3).Range.Text = funcCollection[i].FunctionName;
                table.Cell(i + 2 + validx, 4).Range.Text = funcCollection[i].Summary;

                if (i == 0)
                {
                    table.Cell(2 + validx, 1).Range.Text = "함수";
                }
                else
                {
                    table.Columns[1].Cells[mergeIdx + 1].Merge(table.Columns[1].Cells[mergeIdx]);
                }
            }

            for (int i = 1; i <= 4; i++)
            {
                table.Cell(1, i).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;
            }

            for (int i = 2; i <= table.Columns[1].Cells.Count; i++)
            {
                table.Columns[1].Cells[i].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;
            }
        }

        private void CreateFuncTable(ref Word.Section section, string filePath, FunctionInfo data)
        {
            string fileName = Path.GetFileName(filePath);

            Word.Paragraph funcParagraph = section.Range.Paragraphs.Add();
            funcParagraph.Range.InsertParagraphAfter();

            Word.Table table;
            Word.Range tableRng = word_doc.Bookmarks.get_Item(ref EndOfDoc).Range;
            table = word_doc.Tables.Add(tableRng, 5, 4, ref missing, ref missing);
            table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineWidth = Word.WdLineWidth.wdLineWidth150pt;

            table.Cell(1, 1).Range.Text = "기 능";
            table.Cell(1, 2).Range.Text = data.Summary;

            table.Cell(2, 1).Range.Text = "모함수명";
            table.Cell(2, 2).Range.Text = data.ParentFunctionName;
            table.Cell(2, 3).Range.Text = "소스파일명";
            table.Cell(2, 4).Range.Text = fileName;

            table.Cell(3, 1).Range.Text = "입 력";
            table.Cell(3, 2).Range.Text = string.Join(", ", data.Parameters.Select(p => $"{p.Type} {p.Name}"));
            table.Cell(3, 3).Range.Text = "출 력";
            table.Cell(3, 4).Range.Text = data.ReturnType;

            table.Cell(4, 1).Range.Text = "처   리";

            table.Columns[1].Width = 65;
            table.Columns[2].Width = 154;
            table.Columns[3].Width = 65;
            table.Columns[4].Width = 154;

            for (int i = 1; i < table.Rows.Count; i++)
            {
                //가장 마지막 Row Cell은 정렬안함
                for (int j = 1; j <= table.Columns.Count; j++)
                {
                    table.Cell(i, j).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table.Cell(i, j).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }
            }

            for (int i = 1; i <= table.Rows.Count; i++)
            {
                if (i == 5)
                {
                    table.Rows[i].Range.Cells.Height = 40;
                }
                else
                {
                    table.Rows[i].Range.Cells.Height = 25;
                }

                for (int j = 1; j <= table.Columns.Count; j++)
                {
                    if (i == 1)
                    {
                        if (table.Rows[i].Cells.Count > 2)
                        {
                            table.Rows[i].Cells[3].Merge(table.Rows[i].Cells[2]);
                        }
                    }
                    else if (i == 2 || i == 3)
                    {
                        if (j == 1 || j == 3)
                        {
                            table.Rows[i].Cells[j].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;
                        }
                    }
                    else if (i == 4 || i == 5)
                    {
                        if (table.Rows[i].Cells.Count > 1)
                        {
                            table.Rows[i].Cells[2].Merge(table.Rows[i].Cells[1]);
                        }
                    }
                }
            }

            table.Rows[4].Cells[1].Borders.OutsideLineWidth = Word.WdLineWidth.wdLineWidth150pt;
            table.Rows[1].Cells[1].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;
            table.Rows[4].Cells[1].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;

        }
    }
}
