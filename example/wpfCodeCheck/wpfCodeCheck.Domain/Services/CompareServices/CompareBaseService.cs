using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Local.Helpers;

namespace wpfCodeCheck.Domain.Services.CompareServices
{
    public abstract class CompareBaseService : ICompare
    {
        private readonly ICsvHelper _csvHelper;
        protected IList<FileItem> _code1 = new List<FileItem>();
        protected IList<FileItem> _code2 = new List<FileItem>();
        private IList<SWDetailedItem> _detailStatementItems = new List<SWDetailedItem>();
        protected DiffReulstModel<CompareEntity> _codeCompareModel = new();
        protected CompareBaseService(ICsvHelper csvHelper)
        {
            _csvHelper = csvHelper;
        }
        protected void AddSwDetailItem(FileCompareModel input, FileCompareModel output)
        {
            SWDetailedItem detailedItem = new SWDetailedItem();

            if (input is not null && output is not null)
            {
                detailedItem.을Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 {input.FileIndex} 파일명 {input.FileName}버전 2 크기 {input.FileSize}체크섬 {"0x" + input.Checksum}생성일자 {input.CreateDate} 라인수 {input.LineCount} 기능설명 {input.Description}
                            """;
                detailedItem.으로Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 {output.FileIndex} 파일명 {output.FileName}버전 3 크기 {output.FileSize}체크섬 {"0x" + output.Checksum}생성일자 {output.CreateDate} 라인수 {output.LineCount} 기능설명 {output.Description}
                            """;
                _detailStatementItems.Add(detailedItem);                
            }
            else if(input is not null && output is null)
            {
                detailedItem.을Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 {input.FileIndex} 파일명 {input.FileName}버전 2 크기 {input.FileSize}체크섬 {"0x" + input.Checksum}생성일자 {input.CreateDate} 라인수 {input.LineCount} 기능설명 {input.Description}
                            """;
                detailedItem.으로Content = $"""
                            -
                            """;
                _detailStatementItems.Add(detailedItem);
            }
            else if(input is null && output is not null)
            {
                detailedItem.을Content = $"""
                            -
                            """;
                detailedItem.으로Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 {output.FileIndex} 파일명 {output.FileName}버전 3 크기 {output.FileSize}체크섬 {"0x" + output.Checksum}생성일자 {output.CreateDate} 라인수 {output.LineCount} 기능설명 {output.Description}
                            """;
                _detailStatementItems.Add(detailedItem);
            }
        }
        public abstract Task<List<CompareEntity>> CompareModelCollections(IList<FileCompareModel> inputItems, IList<FileCompareModel> outputItems);

        public async Task<bool> CreateCompareOutputInfo()
        {
            _csvHelper.CreateCSVFile<FileItem>(_code2, "CompareProject");

            await Task.Delay(10);

            _csvHelper.CreateCSVFile<SWDetailedItem>(_detailStatementItems, "Detail");
            await Task.Delay(10);

            return true;
        }


    }
}
