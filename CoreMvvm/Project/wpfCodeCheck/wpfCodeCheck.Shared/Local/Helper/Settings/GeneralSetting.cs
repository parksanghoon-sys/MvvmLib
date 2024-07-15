using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.Shared.Local.Helper.Settings
{
    [Serializable]
    public class GeneralSetting
    {
        [Setting("")]
        public string InputPath { get; set; }
        [Setting("")]
        public string OutputPath { get; set; }
    }
}
