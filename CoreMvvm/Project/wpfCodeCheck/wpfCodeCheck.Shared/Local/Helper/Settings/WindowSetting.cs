using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.Shared.Local.Helper.Settings
{
    public class WindowSetting
    {
        [Setting(500d)]
        public double? XPos
        {
            get;
            set;
        }

        /// <summary>
        /// 창 표시 Y 좌표
        /// </summary>
        [Setting(500d)]
        public double? YPos
        {
            get;
            set;
        }

        /// <summary>
        /// 창 크기 width
        /// </summary>
        [Setting(340d)]
        public double? Width
        {
            get;
            set;
        }

        /// <summary>
        /// 창 크기 height
        /// </summary>
        [Setting(650d)]
        public double? Height
        {
            get;
            set;
        }
    }
}
