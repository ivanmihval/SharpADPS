namespace CoreADPS.Storage.Models
{
    public struct FileSearchResult
    {
        public string Path;
        public bool IsExist;

        public FileSearchResult(string path, bool isExist)
        {
            Path = path;
            IsExist = isExist;
        }
    }
}
