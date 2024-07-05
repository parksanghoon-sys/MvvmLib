using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareEngine
{

    public class CompareEngine
    {
        private CompareText sourceList;
        private CompareText destList;
        private CompareStateList compareStateList;
        private ArrayList matchList;

        public CompareEngine()
        {
            //Left Side to Compare
            sourceList = null;
            //Right Side to Compare            
            destList = null;

            matchList = null;

            compareStateList = null;
        }

        public void StartDiff(CompareText source, CompareText destination)
        {
            if (source.Count() <= 0 && destination.Count() <= 0)
                return;

            sourceList = source;
            destList = destination;

            matchList = new ArrayList();

            compareStateList = new CompareStateList(destination.Count());
            RecursiveComparer(0, destList.Count() - 1, 0, sourceList.Count() - 1);
        }

        private void RecursiveComparer(int destinationStart, int destinationEnd, int sourceStart, int sourceEnd)
        {
            int curBestIndex = -1;
            int curBestLength = -1;

            CompareState currentItem;
            CompareState bestItem = null;

            for (int count = destinationStart; count <= destinationEnd; count++)
            {
                int maxPossibleDestLength = (destinationEnd - count) + 1;

                if (maxPossibleDestLength <= curBestLength)
                    break;

                currentItem = compareStateList.GetByIndex(count);

                if (currentItem.HasValidLength(sourceStart, sourceEnd, maxPossibleDestLength) == false)
                {
                    GetLongestSourceMatch(currentItem, count, destinationEnd, sourceStart, sourceEnd);
                }
                if(currentItem.Status == CompareStatus.Matched)
                {
                    if(currentItem.Length > curBestLength)
                    {
                        curBestIndex = count;
                        curBestLength = currentItem.Length;
                        bestItem = currentItem;
                    }
                    break;
                }
            }
            if (curBestIndex < 0)
            {

            }
            else
            {
                int sourceIndex = 0;
                if (bestItem != null)
                {
                    sourceIndex = bestItem.StartIndex;
                }
                matchList.Add(CompareResultSpan.CreateNoChange(curBestIndex, sourceIndex, curBestLength));

                if(destinationStart < curBestIndex)
                {
                    if(sourceStart < sourceIndex)
                    {
                        RecursiveComparer(destinationStart, curBestIndex - 1, sourceStart, sourceIndex - 1);
                    }
                }
                int upperDestStart = curBestIndex + curBestLength;
                int upperSourceStart = sourceIndex + curBestLength;
                if (destinationEnd > upperDestStart)
                {
                    //we still have more upper dest data
                    if (sourceEnd > upperSourceStart)
                    {
                        //set still have more upper source data
                        // Recursive call to process upper indexes
                        RecursiveComparer(upperDestStart, destinationEnd, upperSourceStart, sourceEnd);
                    }
                }
            }
        }

        private void GetLongestSourceMatch(CompareState curItem, int destIndex, int destEnd, int sourceStart,
                                               int sourceEnd)
        {
            int maxDestLength = (destEnd - destIndex) + 1;

            int curBestLength = 0;
            int curBestIndex = -1;

            for (int sourceIndex = sourceStart; sourceIndex <= sourceEnd; sourceIndex++)
            {
                int maxLength = Math.Min(maxDestLength, (sourceEnd - sourceIndex) + 1);
                if (maxLength <= curBestLength)
                {
                    //No chance to find a longer one any more
                    break;
                }
                int curLength = GetSourceMatchLength(destIndex, sourceIndex, maxLength);
                if (curLength > curBestLength)
                {
                    //This is the best match so far
                    curBestIndex = sourceIndex;
                    curBestLength = curLength;
                }
                //jump over the match
                sourceIndex += curBestLength;
            }
            //DiffState cur = _stateList.GetByIndex(destIndex);
            if (curBestIndex == -1)
            {
                curItem.SetNoMatch();
            }
            else
            {
                curItem.SetMatch(curBestIndex, curBestLength);
            }
        }

        private int GetSourceMatchLength(int destIndex, int sourceIndex, int maxLength)
        {
            int matchCount;
            for (matchCount = 0; matchCount < maxLength; matchCount++)
            {
                if (
                    destList.GetByIndex(destIndex + matchCount).CompareTo(sourceList.GetByIndex(sourceIndex + matchCount)) != 0)
                {
                    break;
                }
            }
            return matchCount;
        }
        public ArrayList DiffResult()
        {
            ArrayList retval = new ArrayList();
            int dcount = destList.Count();
            int scount = sourceList.Count();

            if (dcount == 0)
            {
                if(scount > 0)
                {
                    retval.Add(CompareResultSpan.CreateDeleteSource(0, scount));
                }
                return retval;
            }
            if(scount == 0)
            {
                retval.Add(CompareResultSpan.CreateAddDestination(0, dcount));
            }

            matchList.Sort();
            int curDest = 0;
            int curSource = 0;
            CompareResultSpan last = null;
            foreach(CompareResultSpan drs in matchList)
            {
                if((!AddChanges(retval, curDest, drs.DestinationIndex, curSource, drs.SourceIndex)) &&
                    (last != null))
                {
                    last.AddLength(drs.Length);
                }
                else
                {
                    retval.Add(drs);
                }
                curDest = drs.DestinationIndex + drs.Length;
                curSource = drs.SourceIndex + drs.Length;
                last = drs;
            }
            AddChanges(retval,curDest, dcount, curSource, scount);

            return retval;
        }

        private bool AddChanges(IList report, int curDest, int nextDest, int curSource, int nextSource)
        {
            bool retval = false;
            int diffDest = nextDest - curDest;
            int diffSource = nextSource - curSource;

            if (diffDest > 0)
            {
                if (diffSource > 0)
                {
                    int minDIff = Math.Min(diffDest, diffSource);
                    report.Add(CompareResultSpan.CreateReplace(curDest, curSource, minDIff));
                    if (diffDest > diffSource)
                    {
                        curDest += minDIff;
                        report.Add(CompareResultSpan.CreateAddDestination(curDest, diffDest - diffSource));
                    }
                    else
                    {
                        if (diffSource > diffDest)
                        {
                            curSource += minDIff;
                            report.Add(CompareResultSpan.CreateDeleteSource(curSource, diffSource - diffDest));
                        }
                    }
                }
                else
                {
                    report.Add(CompareResultSpan.CreateAddDestination(curDest, diffDest));
                }
                retval = true;
            }
            else
            {
                if(diffSource > 0)
                {
                    report.Add(CompareResultSpan.CreateDeleteSource(curSource, diffSource));
                    retval = true;
                }   
            }
            return retval;
        }
    }
}
