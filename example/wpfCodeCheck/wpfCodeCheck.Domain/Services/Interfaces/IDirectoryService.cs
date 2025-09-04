using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services.Interfaces
{
    public interface IDirectoryService
    {
        Task<List<FileTreeModel>> GetDirectoryCodeFileInfosAsync(string path);
    }
}
