using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Models
{
    public record class FailClassAnalysisModel()
    {
        public FileTreeModel InputFile { get; init; } = new FileTreeModel();
        public FileTreeModel OutputFile { get; init; } = new FileTreeModel();
        public bool IsSelected { get; set; } = false;
    }
}
