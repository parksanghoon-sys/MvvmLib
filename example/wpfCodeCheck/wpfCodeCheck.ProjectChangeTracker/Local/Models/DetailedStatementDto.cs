using System.ComponentModel;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Models
{
    public class DetailedStatementDto
    {
        public IEnumerable<CompareEntity> CompareResult { get; private set; }
        [Description("제안필요성,타당성")]
        public string Summery { get; set; } = string.Empty;
        [Description("자료명")]
        public string DocumentNumber { get ; set; } = string.Empty;
        [Description("체계 명")]
        public EProjectType ProjectType { get; set; } = EProjectType.MUAV;
        public DetailedStatementDto CompareEntityToDetailedStatementDto(IEnumerable<CompareEntity> compareEntities)
        {
            return new DetailedStatementDto
            {
                Summery = this.Summery,
                DocumentNumber = this.DocumentNumber,
                ProjectType = this.ProjectType,
                CompareResult = compareEntities,

            };
        }
    }
}
