namespace WPFSharpADPS.Helpers
{
    public struct MailViewReposytoryChangedResult
    {
        public string[] NewMessagePaths;
        public string[] DeletedMessagePaths;

        public MailViewReposytoryChangedResult(string[] newMessagePaths = null, string[] deletedMessagePaths = null)
        {
            NewMessagePaths = newMessagePaths ?? new string[0];
            DeletedMessagePaths = deletedMessagePaths ?? new string[0];
        }
    }
}
