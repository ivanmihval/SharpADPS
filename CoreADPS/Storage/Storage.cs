using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CoreADPS.Filters;
using CoreADPS.Helpers;
using CoreADPS.MailModels;
using CoreADPS.Storage.Models;
using CoreADPS.Storage.Models.ProgressModels;

namespace CoreADPS.Storage
{
    public class Storage
    {
        public const string MessagesFolder = "adps_messages";
        public const string AttachmentsFolder = "adps_attachments";
        public const int HashsumFilenamePartLen = 10;
        public const int MessageFileMaxSizeBytes = 4*1024; // 4KB

        public const string DefaultAttachmentExtension = ".bin";
        public const string DefaultMessageExtension = ".json";
        public const string MessageFilenameMask = "*.json";

        public string RootDirPath;

        public delegate void FilterMailsHandler(FilterMailsProgressData filterMailsProgressData);
        public delegate void CopyMailsHandler(CopyMailsProgressData copyMailsProgressData);
        public delegate void DeleteMailsEstimationHandler(DeleteMailsEstimationProgressData deleteMailsEstimationProgressData);

        public ILogger Logger;

        public event FilterMailsHandler FilterMailsHandlerNotify;
        public event CopyMailsHandler CopyMailsHandlerNotify;
        public event DeleteMailsEstimationHandler DeleteMailsEstimationHandlerNotify;

        private void _writeLogWrapper(LoggingLevel level, string message)
        {
            if (Logger != null)
            {
                Logger.Write(level, message);
            }
        }

        public delegate string CalculateHashsum(Stream stream);
        public static string CalculateHashsumByPath(string path, CalculateHashsum calculateHashsum)
        {
            using (var fileStream = new BufferedStream(File.OpenRead(path), 1024 * 1024))
            {
                return calculateHashsum(fileStream);
            }
        }

        public Storage(string rootDirPath)
        {
            RootDirPath = Path.GetFullPath(rootDirPath);
        }

        private static FileSearchResult? _checkPath(string path, string hashsumHex, CalculateHashsum calculateHashsum,
            HashSet<string> cachedAttachmentsFilenames)
        {
            //var fileExists = (cachedAttachmentsFilenames == null || cachedAttachmentsFilenames.Count == 0)
            var fileExists = cachedAttachmentsFilenames == null
                                 ? File.Exists(path)
                // ReSharper disable AssignNullToNotNullAttribute
                                 : cachedAttachmentsFilenames.Contains(path);
            // ReSharper restore AssignNullToNotNullAttribute

            if (!fileExists)
            {
                return new FileSearchResult(path, false);
            }

            if (CalculateHashsumByPath(path, calculateHashsum) == hashsumHex)
            {
                return new FileSearchResult(path, true);
            }

            return null;
        }

        public static void CreateNewRepository(string path)
        {
            // todo: raise custom exception if folders are existed
            Directory.CreateDirectory(Path.Combine(path, AttachmentsFolder));
            Directory.CreateDirectory(Path.Combine(path, MessagesFolder));
        }

        public static bool RepositoryIsValid(string path)
        {
            return Directory.Exists(Path.Combine(path, AttachmentsFolder)) &&
                   Directory.Exists(Path.Combine(path, MessagesFolder));
        }

        public static FileSearchResult GetFreeFilePath(string path, string hashsumHex, CalculateHashsum calculateHashsum,
                                                       HashSet<string> cachedAttachmentsFullPaths = null)
        {
            var result = _checkPath(path, hashsumHex, calculateHashsum, cachedAttachmentsFullPaths);
            if (result.HasValue)
            {
                return result.Value;
            }

            var directory = Path.GetDirectoryName(path);
            if (directory == null)
            {
                throw new NullReferenceException("The root directory is null");
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);

            const int attempts = 10000;
            for (var i = 0; i < attempts; i++)
            {
                var newPath = Path.GetFullPath(Path.Combine(directory,
                                                            String.Format("{0}_{1:D4}{2}", fileNameWithoutExtension, i,
                                                                          extension)));
                result = _checkPath(newPath, hashsumHex, calculateHashsum, cachedAttachmentsFullPaths);
                if (result.HasValue)
                {
                    return result.Value;
                }
            }

            throw new Exception(String.Format("Could not get free path for value for {0}", path));
        }

        public string FindAttachmentPath(string hashsumHex, CalculateHashsum calculateHashsum, 
            HashSet<string> cachedAttachmentsFilenames=null)
        {
            var attachmentsFolderPath = Path.Combine(RootDirPath, AttachmentsFolder);

            var defaultPath = Path.Combine(attachmentsFolderPath,
                                           hashsumHex.Substring(0, HashsumFilenamePartLen) + DefaultAttachmentExtension);

            //return Path.GetFullPath(defaultPath);

            var fileExists = cachedAttachmentsFilenames == null
                                 ? File.Exists(Path.GetFileName(defaultPath))
// ReSharper disable AssignNullToNotNullAttribute
                                 : cachedAttachmentsFilenames.Contains(Path.GetFileName(defaultPath));
// ReSharper restore AssignNullToNotNullAttribute

            var defaultPaths = fileExists ? new List<string> { defaultPath } : new List<string>();

            foreach (var attachmentPath in IterTools.Chain(defaultPaths, () => Directory.EnumerateFiles(attachmentsFolderPath, hashsumHex.Substring(0, HashsumFilenamePartLen) + "*")))
            {
                var calculatedHashsum = CalculateHashsumByPath(attachmentPath, calculateHashsum);
                if (calculatedHashsum == hashsumHex)
                {
                    return Path.GetFullPath(attachmentPath);
                }
            }

            throw new FileNotFoundException(String.Format("Could not find attachment for sha512 {0}", hashsumHex));
        }

        public static Mail LoadMail(string messageFilePath)
        {
            var sizeBytes = new FileInfo(messageFilePath).Length;
            if (sizeBytes > MessageFileMaxSizeBytes)
            {
                throw new Exception(String.Format("The size of the file {0} is more than 4KB", messageFilePath));
            }
            return Mail.FromJson(File.ReadAllText(messageFilePath, Unicode.Utf8WithouBom));
        }

        public string[] GetMsgPaths()
        {
            var messagesFolderPath = Path.Combine(RootDirPath, MessagesFolder);
            return
                Directory.EnumerateFiles(messagesFolderPath, MessageFilenameMask)
                         .Where(p => new FileInfo(p).Length <= MessageFileMaxSizeBytes)
                         .Select(Path.GetFileName)
                         .ToArray();
        }

        public IEnumerable<FilteredMailResult> FilteredMailResults(ICollection<IMailParamFilter> filters, IEnumerable<string> filteredMessageFilenames = null)
        {
            var messagesFolderPath = Path.Combine(RootDirPath, MessagesFolder);
            var messagesPaths = filteredMessageFilenames == null
                                    ? Directory.EnumerateFiles(messagesFolderPath, MessageFilenameMask).ToList()
                                    : filteredMessageFilenames.Select(fmf => Path.Combine(messagesFolderPath, fmf))
                                                              .ToList();

            _writeLogWrapper(LoggingLevel.Info,
                             String.Format("FilteredMailResults: Got {0} messagesPaths", messagesPaths.Count));

            var index = 0;
            foreach (var messagePath in messagesPaths)
            {
                _writeLogWrapper(LoggingLevel.Debug, String.Format("FilteredMailResults: open {0}", messagePath));

                Mail mail;
                try
                {
                    mail = LoadMail(messagePath);
                }
                catch (Exception)
                {
                    continue;
                }

                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("FilteredMailResults: calculate sha512 for {0}", messagePath));

                var hashsumHex = HashsumCalculator.Sha512Checksum(messagePath);
                if (filters == null || mail.IsFiltered(filters))
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("FilteredMailResults: applied filter for {0}", messagePath));
                    yield return new FilteredMailResult(mail, messagePath, hashsumHex);
                }

                if (FilterMailsHandlerNotify != null)
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format("FilteredMailResults: invoke notification for {0}", messagePath));
                    FilterMailsHandlerNotify.Invoke(new FilterMailsProgressData(index, messagesPaths.Count()));
                }

                index++;
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("FilteredMailResults: finishing processing of {0}, index={1}",
                                               messagePath, index));
            }
        }

        public FilteredMailResult FilteredMailResult(string msgFilename)
        {
            var messagesFolderPath = Path.Combine(RootDirPath, MessagesFolder);
            var msgPath = Path.Combine(messagesFolderPath, msgFilename);
            var mail = LoadMail(msgPath);
            var hashsumHex = HashsumCalculator.Sha512Checksum(msgPath);
            return new FilteredMailResult(mail, msgPath, hashsumHex);
        }

        public string SaveMail(Mail mail, List<MailAttachmentInfo> mailAttachmentInfos, CalculateHashsum calculateHashsum)
        {
            var messagesFolderPath = Path.Combine(RootDirPath, MessagesFolder);
            var attachmentsFolderPath = Path.Combine(RootDirPath, AttachmentsFolder);

            var mailJsonBytes = mail.ToJsonBytes();
            var mailJsonHashsumHex = HashsumCalculator.Sha512Checksum(mailJsonBytes);
            _writeLogWrapper(LoggingLevel.Info, String.Format("SaveMail: Got hashsum {0}", mailJsonHashsumHex));

            var messageFilePath = Path.Combine(messagesFolderPath,
                                               mailJsonHashsumHex.Substring(0, HashsumFilenamePartLen) +
                                               DefaultMessageExtension);
            var fileSearchResult = GetFreeFilePath(messageFilePath, mailJsonHashsumHex, calculateHashsum);
            if (!fileSearchResult.IsExist)
            {
                _writeLogWrapper(LoggingLevel.Info,
                                 String.Format("SaveMail: File {0} doesnt exist. Writing...", fileSearchResult.Path));
                File.WriteAllBytes(fileSearchResult.Path, mailJsonBytes);
            }

            foreach (var mailAttachmentInfo in mailAttachmentInfos)
            {
                var attachmentPath = mailAttachmentInfo.Path;
                _writeLogWrapper(LoggingLevel.Debug, String.Format("SaveMail: foreach: processing {0}", attachmentPath));

                var defaultAttachmentPath = Path.Combine(attachmentsFolderPath,
                                                         mailAttachmentInfo.HashsumHex.Substring(0, HashsumFilenamePartLen) + DefaultAttachmentExtension);
                var targetFileSearchResult = GetFreeFilePath(defaultAttachmentPath, mailAttachmentInfo.HashsumHex, calculateHashsum);
                if (!targetFileSearchResult.IsExist)
                {
                    File.Copy(attachmentPath, targetFileSearchResult.Path);
                }
            }

            _writeLogWrapper(LoggingLevel.Info, String.Format("SaveMail: returning {0}", messageFilePath));
            return messageFilePath;
        }

        private static IEnumerable<Tuple<string, string>> GetMessagesFolderAndExtension(string messagesFolderPath)
        {
            while (true)
            {
                yield return new Tuple<string, string>(messagesFolderPath, DefaultMessageExtension);
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        private static IEnumerable<Tuple<string, string>> GetAttachmentsFolderAndExtension(string attachmentsFolderPath)
        {
            while (true)
            {
                yield return new Tuple<string, string>(attachmentsFolderPath, DefaultAttachmentExtension);
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns


        public CopyMailsEstimationResult GetCopyMailsEstimationResult(ICollection<string> msgFilenames, CalculateHashsum calculateHashsum)
        {
            var mailFilesEstimationFileResults = new List<EstimationFileResult>();
            var attachmentFilesEstimationFileResults = new List<EstimationFileResult>();

            var attachmentsFilesHashums = new HashSet<string>();

            var attachmentsFolderPath = Path.Combine(RootDirPath, AttachmentsFolder);
            var cachedAttachmentsFilenames =
                new HashSet<string>(Directory.GetFiles(attachmentsFolderPath).Select(Path.GetFileName));

            var index = 0;
            
            foreach (var msgFilename in msgFilenames)
            {
                _writeLogWrapper(LoggingLevel.Debug, String.Format("GetCopyMailsEstimationResult: foreach: processing {0}", msgFilename));
                var messageFilePath = Path.Combine(RootDirPath, MessagesFolder, msgFilename);
                var mailFileSizeBytes = (UInt64)new FileInfo(messageFilePath).Length;
                var messageFileHashsumHex = HashsumCalculator.Sha512Checksum(messageFilePath);
                mailFilesEstimationFileResults.Add(new EstimationFileResult(messageFilePath, messageFileHashsumHex, mailFileSizeBytes));

                var mail = LoadMail(messageFilePath);
                foreach (var attachment in mail.Attachments)
                {
                    _writeLogWrapper(LoggingLevel.Debug, String.Format("GetCopyMailsEstimationResult: foreach: foreach: processing {0}", attachment.Filename));
                    if (!attachmentsFilesHashums.Contains(attachment.HashsumHex))
                    {
                        _writeLogWrapper(LoggingLevel.Debug, String.Format("GetCopyMailsEstimationResult: foreach: foreach: new attachment {0}", attachment.HashsumHex));
                        attachmentsFilesHashums.Add(attachment.HashsumHex);

                        var attachmentPath = FindAttachmentPath(attachment.HashsumHex, calculateHashsum,
                                                                cachedAttachmentsFilenames);
                        attachmentFilesEstimationFileResults.Add(new EstimationFileResult(attachmentPath,
                                                                                          attachment.HashsumHex,
                                                                                          attachment.SizeBytes));
                        _writeLogWrapper(LoggingLevel.Debug, String.Format("GetCopyMailsEstimationResult: foreach: foreach: found {0}", attachmentPath));
                    }
                }

                if (CopyMailsHandlerNotify != null)
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format("GetCopyMailsEstimationResult: invoke notification for {0}", msgFilename));
                    CopyMailsHandlerNotify.Invoke(new CopyMailsProgressData(CopyMailsStage.Estimation,
                        estimationProgressData: new FilterMailsProgressData(index, msgFilenames.Count)));
                }

                index++;
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetCopyMailsEstimationResult: increment index, new value: {0}", index));
            }

            return new CopyMailsEstimationResult(mailFilesEstimationFileResults, attachmentFilesEstimationFileResults);
        }

        public void CopyMails(CopyMailsEstimationResult copyMailsEstimationResult, string targetFolderPath, CalculateHashsum calculateHashsum)
        {
            var messagesFolderPath = Path.Combine(targetFolderPath, MessagesFolder);
            var attachmentsFolderPath = Path.Combine(targetFolderPath, AttachmentsFolder);

            var cachedTargetRepoFilePaths = new HashSet<string>(
                Directory.GetFiles(attachmentsFolderPath)
                         .Union(Directory.GetFiles(messagesFolderPath))
                         .Select(Path.GetFullPath));

            UInt64 copiedBytes = 0;
            var index = 0;
            foreach (var fileInfo in IterTools.Chain(
                copyMailsEstimationResult.MailFilesEstimationFileResults.Zip(GetMessagesFolderAndExtension(messagesFolderPath), (estimationResult, folderAndExtension) => new { EstimationResult = estimationResult, Folder = folderAndExtension.Item1, Extension = folderAndExtension.Item2 }),
                copyMailsEstimationResult.AttachmentFilesEstimationFileResults.Zip(GetAttachmentsFolderAndExtension(attachmentsFolderPath), (estimationResult, folderAndExtension) => new { EstimationResult = estimationResult, Folder = folderAndExtension.Item1, Extension = folderAndExtension.Item2 })
                )
            )
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("CopyMails: foreach: processing {0}", fileInfo.EstimationResult.Path));
                var fileSearchResult =
                    GetFreeFilePath(
                        Path.GetFullPath(Path.Combine(fileInfo.Folder,
                                                      fileInfo.EstimationResult.HashsumHex.Substring(0,
                                                                                                     HashsumFilenamePartLen) +
                                                      fileInfo.Extension)),
                        fileInfo.EstimationResult.HashsumHex, calculateHashsum, cachedTargetRepoFilePaths);

                if (!fileSearchResult.IsExist)
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format("CopyMails: foreach: Copy new file to {0}", fileSearchResult.Path));
                    File.Copy(fileInfo.EstimationResult.Path, fileSearchResult.Path);
                    cachedTargetRepoFilePaths.Add(fileSearchResult.Path);
                }

                copiedBytes += fileInfo.EstimationResult.SizeBytes;

                if (CopyMailsHandlerNotify != null)
                {
                    _writeLogWrapper(LoggingLevel.Debug, "CopyMails: foreach: invokation notification...");
                    CopyMailsHandlerNotify.Invoke(new CopyMailsProgressData(CopyMailsStage.Copying,
                        copyingProgressData: new CopyMailProgressData(index, fileInfo.EstimationResult.SizeBytes, fileInfo.EstimationResult.Path, copyMailsEstimationResult.TotalFilesNumber, copyMailsEstimationResult.TotalFilesSizeBytes, copiedBytes)));
                }

                index++;

                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("CopyMails: foreach: index {0}, copiedBytes {1}", index, copiedBytes));
            }

            _writeLogWrapper(LoggingLevel.Info, "CopyMails: finished");
        }

        public void CopyMails(ICollection<string> msgFilenames, string targetFolderPath, CalculateHashsum calculateHashsum)
        {
            var estimationResult = GetCopyMailsEstimationResult(msgFilenames, calculateHashsum);
            CopyMails(estimationResult, targetFolderPath, calculateHashsum);
        }

        public List<string> GetAttachmentPathsForDelete(ICollection<string> messageFilenames, CalculateHashsum calculateHashsum)
        {
            if (messageFilenames.Count == 0)
            {
                return new List<string>();
            }

            var messagesFolderPath = Path.Combine(RootDirPath, MessagesFolder);

            var attachmentHashsumsForDelete = new HashSet<string>();
            var attachmentPathByHashsum = new Dictionary<string, string>();
            var messagePathsToDelete = new HashSet<string>();

            var attachmentsFolderPath = Path.Combine(RootDirPath, AttachmentsFolder);
            var cachedAttachmentsFilenames =
                new HashSet<string>(Directory.GetFiles(attachmentsFolderPath).Select(Path.GetFileName));

            var index = 0;
            var messagePathsLength = messageFilenames.Count;
            foreach (var messageFilename in messageFilenames)
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentPathsForDelete: foreach: processing {0}", messageFilename));
                var messageFilePath = Path.Combine(RootDirPath, MessagesFolder, messageFilename);
                messagePathsToDelete.Add(Path.GetFullPath(messageFilePath));

                var mail = LoadMail(messageFilePath);
                foreach (var attachment in mail.Attachments)
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format("GetAttachmentPathsForDelete: foreach: foreach: processing {0}",
                                                   attachment.Filename));

                    if (!attachmentHashsumsForDelete.Contains(attachment.HashsumHex))
                    {
                        _writeLogWrapper(LoggingLevel.Debug,
                                         String.Format(
                                             "GetAttachmentPathsForDelete: foreach: foreach: new file with hashsum {0}",
                                             attachment.HashsumHex));

                        try
                        {
                            var attachmentPath = FindAttachmentPath(attachment.HashsumHex, calculateHashsum, cachedAttachmentsFilenames);
                            attachmentHashsumsForDelete.Add(attachment.HashsumHex);
                            attachmentPathByHashsum[attachment.HashsumHex] = attachmentPath;
                        }
                        catch (FileNotFoundException)
                        {
                        }
                    }
                }

                if (DeleteMailsEstimationHandlerNotify != null)
                {
                    _writeLogWrapper(LoggingLevel.Debug, "GetAttachmentPathsForDelete: foreach: invoke notification");
                    DeleteMailsEstimationHandlerNotify.Invoke(new DeleteMailsEstimationProgressData(DeleteMailsEstimationStage.ScanningTargetFiles, index, messagePathsLength));
                }

                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentPathsForDelete: foreach: index {0}", index));
                index++;
            }

            _writeLogWrapper(LoggingLevel.Debug, "GetAttachmentPathsForDelete: exit foreach");

            if (attachmentHashsumsForDelete.Count == 0)
            {
                _writeLogWrapper(LoggingLevel.Info, "GetAttachmentPathsForDelete: attachmentHashsumsForDelete.Count == 0");
                return new List<string>();
            }

            var allMessagesPaths = Directory.EnumerateFiles(messagesFolderPath, MessageFilenameMask).ToList();
            var allMessagesCount = allMessagesPaths.Count;
            index = 0;
            foreach (var messagePath in allMessagesPaths)
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentPathsForDelete: foreach2: path {0}", messagePath));

                if (DeleteMailsEstimationHandlerNotify != null)
                {
                    _writeLogWrapper(LoggingLevel.Debug, "GetAttachmentPathsForDelete: foreach2: invoke notification");
                    DeleteMailsEstimationHandlerNotify.Invoke(new DeleteMailsEstimationProgressData(DeleteMailsEstimationStage.ScanningAllFiles, index, allMessagesCount));
                }

                if (messagePathsToDelete.Contains(Path.GetFullPath(messagePath)))
                {
                    continue;
                }

                var mail = LoadMail(messagePath);
                foreach (var attachment in mail.Attachments)
                {
                    if (attachmentHashsumsForDelete.Contains(attachment.HashsumHex))
                    {
                        _writeLogWrapper(LoggingLevel.Debug,
                                         String.Format(
                                             "GetAttachmentPathsForDelete: foreach2: foreach: found already linked file, removing {0} from the deletion list",
                                             attachment.HashsumHex));
                        attachmentHashsumsForDelete.Remove(attachment.HashsumHex);
                    }
                }

                index++;
            }

            _writeLogWrapper(LoggingLevel.Info, "Exiting GetAttachmentPathsForDelete");
            return attachmentHashsumsForDelete.Select(hashsum => attachmentPathByHashsum[hashsum]).ToList();
        }

        public List<Tuple<string, string>> GetAttachmentRenameMapping(
            IEnumerable<string> cachedAttachmentsFilenames = null)
        {
            var attachmentsFolderPath = Path.Combine(RootDirPath, AttachmentsFolder);
            var attachmentsFilenames = cachedAttachmentsFilenames == null
                                           ? Directory.GetFiles(attachmentsFolderPath)
                                                      .Select(Path.GetFileName)
                                                      .ToArray()
                                           : cachedAttachmentsFilenames.ToArray();
            var partialHashsumsWithCollisions = new HashSet<string>();
            var hexChars = new HashSet<char>("0123456789abcdef".ToCharArray());
            foreach (var attachmentFilename in attachmentsFilenames)
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentRenameMapping: foreach: {0}", attachmentFilename));
                if (!attachmentFilename.Contains('_'))
                {
                    continue;
                }
                var partialHashsumPart = attachmentFilename.Substring(0, HashsumFilenamePartLen);
                if (partialHashsumPart.ToCharArray().Select(hexChars.Contains).All(b=>b))
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format(
                                         "GetAttachmentRenameMapping: foreach: {0} has mutual partial hashsum",
                                         attachmentFilename));
                    partialHashsumsWithCollisions.Add(partialHashsumPart);
                }
            }

            if (!partialHashsumsWithCollisions.Any())
            {
                _writeLogWrapper(LoggingLevel.Info, "GetAttachmentRenameMapping: No partial hashsums, return empty list");
                return new List<Tuple<string, string>>();
            }

            var filenamesByPartialHashsum = new Dictionary<string, List<string>>();
            foreach (var attachmentFilename in attachmentsFilenames)
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentRenameMapping: foreach2: {0}", attachmentFilename));
                var partialHashsumPart = attachmentFilename.Substring(0, HashsumFilenamePartLen);
                if (partialHashsumsWithCollisions.Contains(partialHashsumPart))
                {
                    if (filenamesByPartialHashsum.ContainsKey(partialHashsumPart))
                    {
                        filenamesByPartialHashsum[partialHashsumPart].Add(attachmentFilename);
                    }
                    else
                    {
                        filenamesByPartialHashsum[partialHashsumPart] = new List<string>{attachmentFilename};
                    }
                }
            }

            var result = new List<Tuple<string, string>>();
            foreach (var partialHashsum in filenamesByPartialHashsum.Keys)
            {
                _writeLogWrapper(LoggingLevel.Debug,
                                 String.Format("GetAttachmentRenameMapping: foreach3: {0}", partialHashsum));

                var initSortedFilenames = filenamesByPartialHashsum[partialHashsum].OrderBy(s => s).ToList();

                var resultedSortedFilenames = new List<string>(initSortedFilenames.Count);
                for (var i = 0; i < initSortedFilenames.Count; i++)
                {
                    var filename = i == 0
                                       ? String.Format("{0}.bin", partialHashsum)
                                       : String.Format("{0}_{1:D4}.bin", partialHashsum, i - 1);
                    resultedSortedFilenames.Add(filename);
                }

                if (!initSortedFilenames.SequenceEqual(resultedSortedFilenames))
                {
                    _writeLogWrapper(LoggingLevel.Debug,
                                     String.Format(
                                         "GetAttachmentRenameMapping: foreach3: files with partial hashsum {0} are needed to be renamed",
                                         partialHashsum));
                    foreach (
                        var resultItem in
                            initSortedFilenames.Zip(resultedSortedFilenames,
                                                    (s1, s2) => new {before = s1, after = s2}))
                    {
                        if (resultItem.before != resultItem.after)
                        {
                            result.Add(new Tuple<string, string>(Path.Combine(attachmentsFolderPath, resultItem.before),
                                                                 Path.Combine(attachmentsFolderPath, resultItem.after)));
                        }
                    }
                }
            }

            _writeLogWrapper(LoggingLevel.Info, "GetAttachmentRenameMapping: returning result");
            return result;
        }
    }
}
