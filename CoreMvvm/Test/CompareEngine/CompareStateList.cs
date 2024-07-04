
namespace CompareEngine
{
    internal class CompareStateList
    {
        private readonly CompareState[] compareStateArray;
        public CompareStateList(int destinationCount)
        {
            compareStateArray = new CompareState[destinationCount];
        }

        internal CompareState GetByIndex(int count)
        {
            CompareState compareState = compareStateArray[count];
            if (compareState == null)
            {
                compareState = new CompareState();
                compareStateArray[count] = compareState;
            }
            return compareState;
        }
    }
}