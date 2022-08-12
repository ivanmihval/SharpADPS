namespace CoreADPS.Storage.Models.ProgressModels
{
    public struct FilterMailsProgressData
    {
        public int CurrentIndex;
        public int TotalMailsNumber;

        public FilterMailsProgressData(int currentIndex, int totalMailsNumber)
        {
            CurrentIndex = currentIndex;
            TotalMailsNumber = totalMailsNumber;
        }
    }
}
