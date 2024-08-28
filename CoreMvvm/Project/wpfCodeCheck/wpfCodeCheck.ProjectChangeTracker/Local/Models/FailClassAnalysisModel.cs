using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Models
{
    public record class FailClassAnalysisModel()
    {
        public FileEntity InputFile { get; init; } = new FileEntity();
        public FileEntity OutputFile { get; init; } = new FileEntity();
        public bool IsSelected { get; set; } = false;
    }

}
