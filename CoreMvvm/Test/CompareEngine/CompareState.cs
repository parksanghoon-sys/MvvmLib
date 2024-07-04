using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareEngine
{
    internal class CompareState
    {
        private const int BAD_INDEX = -1;
        private int _length;
        private int _startIndex;
        public CompareState()
        {
            _startIndex = BAD_INDEX;
            _length = (int)CompareStatus.Unknown;
        }
        public int StartIndex
        {
            get { return _startIndex; }
        }

        public int EndIndex
        {
            get { return ((_startIndex + _length) - 1); }
        }
        public int Length
        {
            get
            {
                int len;
                if(_length > 0)
                    len = _length;
                else
                    len  = _length == 0 ? 1 : 0;
                return len;
            }
        }
        public CompareStatus Status
        {
            get
            {
                CompareStatus status;
                if (_length > 0)
                    status = CompareStatus.Matched;
                else
                {
                    switch(_length)
                    {
                        case -1:
                            status = CompareStatus.NoMatch;
                            break;
                        default:
                            status = CompareStatus.Unknown;
                            break;
                    }
                }
                return status;
            }
        }
        protected void SetToUnkown()
        {
            _startIndex = BAD_INDEX;
            _length = (int)CompareStatus.Unknown;
        }
        internal bool HasValidLength(int sourceStart, int sourceEnd, int maxPossibleDestLength)
        {
            if(_length > 0)
            {
                if((maxPossibleDestLength < _length ) || ((_startIndex < sourceStart) ||(EndIndex > sourceEnd)))
                {
                    SetToUnkown();
                }
            }
            return (_length != (int)CompareStatus.Unknown);
        }

        public void SetMatch(int startIndex, int legth)
        {
            _startIndex = startIndex;
            _length = legth;
        }

        public void SetNoMatch()
        {
            _startIndex = BAD_INDEX;
            _length = (int)CompareStatus.NoMatch;
        }
    }
}
