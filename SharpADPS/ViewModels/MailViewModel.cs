using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CoreADPS.Helpers;
using CoreADPS.MailModels;
using CoreADPS.Storage;
using CoreADPS.Storage.Models;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Helpers.FolderBrowserDialogProvider;
using WPFSharpADPS.Helpers.MessageBoxProvider;
using WPFSharpADPS.Helpers.OpenFileDialogProvider;
using WPFSharpADPS.Models;
using WPFSharpADPS.Models.Translation;

namespace WPFSharpADPS.ViewModels
{
    public enum MailViewModelMode
    {
        New,
        ViewExisting,
        EditExisting,
    }

    public class MailViewModel : PropertyChangedModel
    {
        private readonly HashSet<string> _savedMessagesSha512 = new HashSet<string>();
        private readonly Dictionary<string, string> _pathBySha512 = new Dictionary<string, string>();

        private string _messageFilename;
        private readonly string _storagePath;
        private Mail _mail;
        private readonly Storage _storage;

        private string _mailAdditionalNotes;
        private string _mailInlineMessage;

        private List<AttachmentViewObject> _attachmentViewObjects;
        private bool _isAttachmentsListBoxSelected;

        private List<LocationViewObject> _locationViewObjects;
        private bool _isCoordinatesListBoxSelected;

        private ulong _progressBarValue;
        private ulong _maxProgressBarValue = 100;
        private bool _isProgressBarVisible;
        private string _progressBarText;
        private bool _isProgressbarIndeterminate;

        private bool _datetimeCreatedIsNow;

        private string _coordinatesTextBoxContent;

        private MailViewModelMode _mailViewModelMode;

        public IMessageBoxProvider MessageBoxProvider = new WinFormsMessageBoxProvider();
        public IOpenFileDialogProvider OpenFileDialogProvider = new WinFormsOpenFileDialogProvider();
        public IFolderBrowserDialogProvider FolderBrowserDialogProvider = new WinFormsFolderBrowserDialogProvider();

        public delegate void RepositoryUpdatedHandler(MailViewReposytoryChangedResult result);
        public event RepositoryUpdatedHandler RepositoryUpdatedHandlerNotify;

        public Window ThisWindow;

        public ICommand ClickAddAttachmentButtonCommand { get; set; }
        public ICommand ClickDeleteAttachmentButtonCommand { get; set; }

        public ICommand ClickAddCoordinatesButtonCommand { get; set; }
        public ICommand ClickDeleteCoordinatesButtonCommand { get; set; }

        public ICommand ClickChangeLockStatusButtonCommand { get; set; }
        public ICommand ClickCloseButtonCommand { get; set; }
        public ICommand ClickExportToFolderButtonCommand { get; set; }
        public ICommand ClickSaveMessageButtonCommand { get; set; }

        private void _onChangedMail()
        {
            OnPropertyChanged("Mail");
            OnPropertyChanged("JsonBytesLength");
            OnPropertyChanged("MessageFileIsTooBig");
        }

        public TranslationViewObject TranslationViewObject
        {
            get { return TranslationViewObject.LoadFromSettings(); }
        }

        public string MessageFilename
        {
            get { return _messageFilename; }
            set
            {
                _messageFilename = value;
                OnPropertyChanged("MessageFileName");
            }
        }

        public string ProgressBarText
        {
            get { return _progressBarText; }
            set
            {
                _progressBarText = value;
                OnPropertyChanged("ProgressBarText");
            }
        }

        public ulong ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        public ulong MaxProgressBarValue
        {
            get { return _maxProgressBarValue; }
            set
            {
                _maxProgressBarValue = value;
                OnPropertyChanged("MaxProgressBarValue");
            }
        }

        public bool IsProgressbarVisible
        {
            get { return _isProgressBarVisible; }
            set
            {
                _isProgressBarVisible = value;
                OnPropertyChanged("IsProgressbarVisible");
            }
        }

        public bool IsProgressbarIndeterminate
        {
            get { return _isProgressbarIndeterminate; }
            set
            {
                _isProgressbarIndeterminate = value;
                OnPropertyChanged("IsProgressbarIndeterminate");
            }
        }

        public DateTime DateCreatedDate
        {
            get { return Mail.DateCreated.Date; }
            set
            {
                Mail.DateCreated = new DateTime(value.Year, value.Month, value.Day,
                                                Mail.DateCreated.Hour, Mail.DateCreated.Minute, 0);

                _onChangedMail();
            }
        }

        public List<string> Hours
        {
            get
            {
                return
                    new List<string>(
                        Enumerable.Range(0, 24).Select(n => n.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0')));
            }
        }

        public string DateCreatedHour
        {
            get { return Mail.DateCreated.Hour.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0'); }
            set
            {
                int hour;
                var parsed = int.TryParse(value, out hour);
                if (parsed)
                {
                    Mail.DateCreated = new DateTime(Mail.DateCreated.Year, Mail.DateCreated.Month, Mail.DateCreated.Day,
                                                    hour, Mail.DateCreated.Minute, 0);

                    _onChangedMail();
                }
            }
        }

        public List<string> Minutes
        {
            get
            {
                return
                    new List<string>(
                        Enumerable.Range(0, 60).Select(n => n.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0')));
            }
        }

        public string DateCreatedMinute
        {
            get { return Mail.DateCreated.Minute.ToString(NumberFormatInfo.InvariantInfo).PadLeft(2, '0'); }
            set
            {
                int minute;
                var parsed = int.TryParse(value, out minute);
                if (parsed)
                {
                    Mail.DateCreated = new DateTime(Mail.DateCreated.Year, Mail.DateCreated.Month, Mail.DateCreated.Day,
                                                    Mail.DateCreated.Hour, minute, 0);

                    _onChangedMail();
                }
            }
        }

        public Mail Mail
        {
            get { return _mail; }
            set 
            {
                _mail = value;

                _onChangedMail();
            }
        }

        public int JsonBytesLength
        {
            get
            {
                return Mail == null ? 0 : Mail.ToJsonBytes().Length;
            }
        }

        public int MaxJsonBytes { get { return Storage.MessageFileMaxSizeBytes; } }

        public bool MessageFileIsTooBig { get { return JsonBytesLength > MaxJsonBytes; } }

        public string MailName
        {
            get { return Mail.Name; }
            set
            {
                Mail.Name = value; 

                OnPropertyChanged("MailName");
                _onChangedMail();
            }
        }

        public string MailAdditionalNotes
        {
            get { return _mailAdditionalNotes; }
            set
            {
                Mail.AdditionalNotes = value;
                _mailAdditionalNotes = value;

                OnPropertyChanged("MailAdditionalNotes");
                _onChangedMail();
            }
        }

        public bool MailAdditionalNotesIsNull
        {
            get { return Mail.AdditionalNotes == null; }
            set 
            { 
                if (value)
                {
                    Mail.AdditionalNotes = null;
                }
                else
                {
                    if (MailAdditionalNotes == null)
                    {
                        _mailAdditionalNotes = "";
                    }
                    Mail.AdditionalNotes = _mailAdditionalNotes;
                }

                OnPropertyChanged("MailAdditionalNotes");
                OnPropertyChanged("MailAdditionalNotesIsNull");
                _onChangedMail();
            }
        }

        public string MailInlineMessage
        {
            get { return _mailInlineMessage; }
            set
            {
                Mail.InlineMessage = value;
                _mailInlineMessage = value;
                OnPropertyChanged("MailMailInlineMessage");
                _onChangedMail();
            }
        }

        public bool MailInlineMessageIsNull
        {
            get { return Mail.InlineMessage == null; }
            set
            {
                if (value)
                {
                    Mail.InlineMessage = null;
                }
                else
                {
                    if (MailInlineMessage == null)
                    {
                        _mailInlineMessage = "";
                    }
                    Mail.InlineMessage = _mailInlineMessage;
                }

                OnPropertyChanged("MailInlineMessage");
                OnPropertyChanged("MailInlineMessageIsNull");
                _onChangedMail();
            }
        }

        public List<AttachmentViewObject> MailAttachments
        {
            get { return _attachmentViewObjects; }
            set
            {
                _attachmentViewObjects = value;
                Mail.Attachments = value.Select(aObj => aObj.Attachment).ToList();
                OnPropertyChanged("MailAttachments");

                _onChangedMail();
            }
        }

        public bool IsAttachmentsListBoxSelected
        {
            get { return _isAttachmentsListBoxSelected; }
            set
            {
                _isAttachmentsListBoxSelected = value;
                OnPropertyChanged("IsAttachmentsListBoxSelected");
            }
        }

        public List<LocationViewObject> LocationViewObjects
        {
            get { return _locationViewObjects; }
            set
            {
                _locationViewObjects = value;
                Mail.RecipientsCoordinates = value.Select(lObj => lObj.Coordinates).ToList();
                OnPropertyChanged("LocationViewObjects");

                _onChangedMail();
            }
        }

        public bool IsCoordinatesListBoxSelected
        {
            get { return _isCoordinatesListBoxSelected; }
            set
            {
                _isCoordinatesListBoxSelected = value;
                OnPropertyChanged("IsCoordinatesListBoxSelected");
            }
        }

        public bool DatetimeCreatedIsNow
        {
            get { return _datetimeCreatedIsNow; }
            set
            {
                _datetimeCreatedIsNow = value;
                if (value)
                {
                    Mail.DateCreated = DateTimeGenerator.UtcNow();
                }

                OnPropertyChanged("DatetimeCreatedIsNow");

                OnPropertyChanged("DateCreatedHour");
                OnPropertyChanged("DateCreatedMinute");
                OnPropertyChanged("DateCreatedDate");
                _onChangedMail();
            }
        }

        public string CoordinatesTextBoxContent
        {
            get { return _coordinatesTextBoxContent; }
            set
            {
                _coordinatesTextBoxContent = value;
                OnPropertyChanged("CoordinatesTextBoxContent");
            }
        }

        public MailViewModelMode MailViewModelMode
        {
            get { return _mailViewModelMode; }
            set
            {
                _mailViewModelMode = value;
                OnPropertyChanged("MailViewModelMode");
            }
        }

        public void InitProperties()
        {
            MailName = Mail.Name;
            MailAdditionalNotes = Mail.AdditionalNotes;
            MailInlineMessage = Mail.InlineMessage;
            MailAttachments = new List<AttachmentViewObject>(Mail.Attachments.Select(a => new AttachmentViewObject(a)));
            LocationViewObjects = new List<LocationViewObject>(Mail.RecipientsCoordinates.Select(c => new LocationViewObject(c)));

            ClickAddAttachmentButtonCommand = new CommandHandler(_addAttachment, _ => true);
            ClickDeleteAttachmentButtonCommand = new CommandHandler(_deleteAttachment, _canDeleteAttachment);

            ClickAddCoordinatesButtonCommand = new CommandHandler(_addCoordinates, _canAddCoordinates);
            ClickDeleteCoordinatesButtonCommand = new CommandHandler(_deleteCoordinates, _canDeleteCoordinates);

            ClickChangeLockStatusButtonCommand = new CommandHandler(_changeLockStatus, _canChangeLockStatus);
            ClickCloseButtonCommand = new CommandHandler(_closeWindow, _canCloseWindow);
            ClickExportToFolderButtonCommand = new CommandHandler(_exportToFolder, _canExportToFolder);
            ClickSaveMessageButtonCommand = new CommandHandler(_saveMessage, _canSaveMessage);
        }

        public MailViewModel(string messagePath, string storagePath)
        {
            _messageFilename = Path.GetFileName(messagePath);

            _storagePath = storagePath;
            _storage = new Storage(_storagePath);

            var filteredMailResult = _storage.FilteredMailResult(_messageFilename);

            Mail = filteredMailResult.Mail;
            _savedMessagesSha512.Add(HashsumCalculator.Sha512Checksum(messagePath));
            _savedMessagesSha512.Add(HashsumCalculator.Sha512Checksum(Mail.ToJsonBytes()));

            MailViewModelMode = MailViewModelMode.ViewExisting;

            InitProperties();
        }

        public MailViewModel(string storagePath = null)
        {
            //_messageFileName = "e882cef10f.json";
            //_storagePath = "C:\\downloads\\adps_repo";
            //_storage = new Storage(_storagePath);

            //_filteredMailResult = _storage.FilteredMailResult(_messageFileName);
            _storagePath = storagePath;
            _storage = new Storage(_storagePath);

            Mail = Mail.EmptyMail();
            MailViewModelMode = MailViewModelMode.New;

            InitProperties();
        }

        public MailViewModel()
        {
            //_storagePath = null;
            //_storage = new Storage(_storagePath);

            Mail = Mail.EmptyMail();
            MailViewModelMode = MailViewModelMode.New;

            InitProperties();
        }

        public void Delete()
        {
            // todo: delete
            // todo: Dispose and close the window
            if (RepositoryUpdatedHandlerNotify != null)
            {
                RepositoryUpdatedHandlerNotify.Invoke(new MailViewReposytoryChangedResult(new string[0], new string[0]));
            }
        }

        private void _onUpdatedProgressStream(object sender, ProgressEventArgs eventArgs)
        {
            IsProgressbarVisible = true;
            ProgressBarText = TranslationViewObject.MailProgressBarCalculatingHashsum;
            MaxProgressBarValue = eventArgs.Length;
            ProgressBarValue = eventArgs.Position;
        }

        private void _onUpdatedProgressStreamBackground(object sender, DoWorkEventArgs e)
        {
            var path = (string) e.Argument;
            var sizeBytes = (ulong) new FileInfo(path).Length;


            //var hashsum = HashsumCalculator.Sha512ChecksumOpenssl(path);
            //e.Result = new Attachment(Path.GetFileName(path), sizeBytes, hashsum);
            //_pathBySha512[hashsum] = path;

            using (var source = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
            {
                using (var progressStream = new ProgressStream(source))
                {
                    progressStream.UpdateProgress += _onUpdatedProgressStream;
                    var hashsum = SettingsManager.SettingsManager.GetHashsumEngine().Calculate(progressStream); //HashsumCalculator.Sha512ChecksumOpenssl(progressStream);
                    e.Result = new Attachment(Path.GetFileName(path), sizeBytes, hashsum);

                    _pathBySha512[hashsum] = path;
                }
            }
        }

        private void _onCompletedProgressStream(object sender, RunWorkerCompletedEventArgs e)
        {
            var attachment = (Attachment) e.Result;

            MailAttachments =
                new List<AttachmentViewObject>(
                    IterTools.Chain(
                        Mail.Attachments.Where(a => a.Filename != attachment.Filename)
                            .Select(a => new AttachmentViewObject(a)),
                        new [] {new AttachmentViewObject(attachment)}));
            //Mail.Attachments.Add(attachment);

            IsProgressbarVisible = false;
            ProgressBarValue = 0;
        }

        private void _addAttachment(object o = null)
        {
            string path;
            var isFileChoosen = OpenFileDialogProvider.ChooseFile(out path);
            if (isFileChoosen)
            {
                var worker = new BackgroundWorker();

                worker.DoWork += _onUpdatedProgressStreamBackground;
                worker.RunWorkerCompleted += _onCompletedProgressStream;
                worker.RunWorkerAsync(argument: path);
            }
        }

        private bool _canDeleteAttachment(object o = null)
        {
            var selectedItem = _getSelectedAttachmentViewObject();
            return (selectedItem != null) && IsAttachmentsListBoxSelected;
        }

        private AttachmentViewObject _getSelectedAttachmentViewObject()
        {
            if (MailAttachments == null)
            {
                return null;
            }
            return MailAttachments.FirstOrDefault(aObj => aObj.IsSelected);
        }

        private void _deleteAttachment(object o = null)
        {
            var selectedItem = _getSelectedAttachmentViewObject();
            if (selectedItem == null)
            {
                return;
            }

            MailAttachments =
                new List<AttachmentViewObject>(
                    Mail.Attachments.Where(
                        a => a.Filename.ToLowerInvariant() != selectedItem.Attachment.Filename.ToLowerInvariant())
                        .Select(a => new AttachmentViewObject(a)));
        }



        private bool _canDeleteCoordinates(object o = null)
        {
            var selectedItem = _getSelectedLocationViewObject();
            return (selectedItem != null) && IsCoordinatesListBoxSelected;
        }

        private LocationViewObject _getSelectedLocationViewObject()
        {
            if (LocationViewObjects == null)
            {
                return null;
            }
            return LocationViewObjects.FirstOrDefault(lObj => lObj.IsSelected);
        }

        private void _deleteCoordinates(object o = null)
        {
            var selectedItem = _getSelectedLocationViewObject();
            if (selectedItem == null)
            {
                return;
            }

            LocationViewObjects =
                new List<LocationViewObject>(
                    Mail.RecipientsCoordinates.Where(c => !c.Equals(selectedItem.Coordinates))
                        .Select(c => new LocationViewObject(c)));
        }

        private Coordinates _getEnteredCoordinates()
        {
            return Coordinates.FromString(CoordinatesTextBoxContent);
        }

        private bool _canAddCoordinates(object o = null)
        {
            return _getEnteredCoordinates() != null;
        }

        private void _addCoordinates(object o = null)
        {
            var value = _getEnteredCoordinates();
            if (value == null)
            {
                return;
            }

            LocationViewObjects =
                new List<LocationViewObject>(
                    IterTools.Chain(LocationViewObjects.Where(lObj => !lObj.Coordinates.Equals(value)),
                                    new [] {new LocationViewObject(value)}));
        }

        private bool _canChangeLockStatus(object o = null)
        {
            return (MailViewModelMode == MailViewModelMode.EditExisting)
                   || (MailViewModelMode == MailViewModelMode.ViewExisting);
        }

        private void _changeLockStatus(object o = null)
        {
            if (MailViewModelMode == MailViewModelMode.EditExisting)
            {
                MailViewModelMode = MailViewModelMode.ViewExisting;
            }
            else if (MailViewModelMode == MailViewModelMode.ViewExisting)
            {
                MailViewModelMode = MailViewModelMode.EditExisting;
            }
        }

        private bool _canCloseWindow(object o = null)
        {
            return ThisWindow != null;
        }

        private void _closeWindow(object o = null)
        {
            ThisWindow.Close();
        }

        private static Dictionary<Tuple<string, string>, List<string>> _buildFilenamesByHashsum(Mail mail, CopyMailsEstimationResult copyMailsEstimationResult)
        {
            var result = new Dictionary<Tuple<string, string>, List<string>>();

            var mailHashsumKey =
                new Tuple<string, string>(copyMailsEstimationResult.MailFilesEstimationFileResults[0].HashsumHex,
                                          copyMailsEstimationResult.MailFilesEstimationFileResults[0].HashsumAlgorithm);
            result[mailHashsumKey] = new List<string> {Path.GetFileName(copyMailsEstimationResult.MailFilesEstimationFileResults[0].Path)};

            foreach (var attachment in mail.Attachments)
            {
                var hashsumKey = new Tuple<string, string>(attachment.HashsumHex, attachment.HashsumAlgorithm);
                if (result.ContainsKey(hashsumKey))
                {
                    result[hashsumKey].Add(attachment.Filename);
                }
                else
                {
                    result[hashsumKey] = new List<string> {attachment.Filename};
                }
            }

            return result;
        }

        private Dictionary<Tuple<string, string>, string> _buildSourcePathByHashsum(
            CopyMailsEstimationResult copyMailsEstimationResult)
        {
            var result = new Dictionary<Tuple<string, string>, string>();

            foreach (
                var estimationResult in
                    IterTools.Chain(copyMailsEstimationResult.MailFilesEstimationFileResults,
                                    copyMailsEstimationResult.AttachmentFilesEstimationFileResults))
            {
                var hashsumKey = new Tuple<string, string>(estimationResult.HashsumHex, estimationResult.HashsumAlgorithm);
                result[hashsumKey] = estimationResult.Path;
            }

            return result;
        }

        private static ulong _getTotalBytesToExport(IDictionary<Tuple<string, string>, List<string>> filenamesByHashsum,
                                                    CopyMailsEstimationResult copyMailsEstimationResult)
        {
            ulong totalBytes = 0;
            foreach (
                var estimationResult in
                    IterTools.Chain(copyMailsEstimationResult.AttachmentFilesEstimationFileResults,
                                    copyMailsEstimationResult.MailFilesEstimationFileResults))
            {
                var hashsumKey = new Tuple<string, string>(estimationResult.HashsumHex,
                                                           estimationResult.HashsumAlgorithm);
                totalBytes += estimationResult.SizeBytes*(ulong) filenamesByHashsum[hashsumKey].Count;
            }

            return totalBytes;
        }

        private bool _canExportToFolder(object o = null)
        {
            return MailViewModelMode == MailViewModelMode.ViewExisting;
        }

        private void _exportToFolder(object o = null)
        {
            string targetFolder;
            if (!FolderBrowserDialogProvider.ChooseFolder(out targetFolder))
            {
                return;
            }

            var folderIsEmpty = Directory.GetFiles(targetFolder).Length == 0;
            if (!folderIsEmpty)
            {
                MessageBoxProvider.Show(TranslationViewObject.MailMessageBoxFolderNotEmpty, TranslationViewObject.OverviewMessageBoxErrorCaption);
                return;
            }

            var worker = new BackgroundWorker();
            //_exportToFolderBackground(null, new DoWorkEventArgs(targetFolder));

            worker.DoWork += _exportToFolderBackground;
            worker.RunWorkerAsync(argument: targetFolder);
        }

        private void _exportToFolderBackground(object sender, DoWorkEventArgs e)
        {
            IsProgressbarVisible = true;
            var targetFolder = (string) e.Argument;
            IsProgressbarIndeterminate = true;
            ProgressBarText = TranslationViewObject.MailProgressBarPreparingToCopy;

            var estimationResults =
                _storage.GetCopyMailsEstimationResult(new[] {_messageFilename},
                                                      SettingsManager.SettingsManager.GetHashsumEngine().Calculate);

            var filenamesByHashsum = _buildFilenamesByHashsum(_mail, estimationResults);
            var totalSizeBytes = _getTotalBytesToExport(filenamesByHashsum, estimationResults);
            var sourcePathByHashsum = _buildSourcePathByHashsum(estimationResults);

            IsProgressbarIndeterminate = false;
            ProgressBarText = TranslationViewObject.MailProgressBarCopyingFiles;

            ProgressBarValue = 0;
            MaxProgressBarValue = totalSizeBytes;

            ulong bytesCopied = 0;
            foreach (var hashsumKey in sourcePathByHashsum.Keys)
            {
                var sourcePath = sourcePathByHashsum[hashsumKey];
                var sizeBytes = (ulong) new FileInfo(sourcePath).Length;
                foreach (var filename in filenamesByHashsum[hashsumKey])
                {
                    var targetPath = Path.Combine(targetFolder, filename);
                    File.Copy(sourcePath, targetPath);
                    bytesCopied += sizeBytes;
                    ProgressBarValue = bytesCopied;
                }
            }

            IsProgressbarVisible = false;
            MessageBoxProvider.Show(TranslationViewObject.MailMessageBoxOperationFinished, TranslationViewObject.OverviewMessageBoxNotificationCaption);
        }

        private string _calculateMessageJsonSha512()
        {
            return HashsumCalculator.Sha512Checksum(Mail.ToJsonBytes());
        }

        private bool _canSaveMessage(object o = null)
        {
            if ((MailViewModelMode != MailViewModelMode.New) 
                && (MailViewModelMode != MailViewModelMode.EditExisting))
            {
                return false;
            }

            if (_savedMessagesSha512.Contains(_calculateMessageJsonSha512()))
            {
                return false;
            }

            if (JsonBytesLength > MaxJsonBytes)
            {
                return false;
            }

            return true;
        }

        private void _saveMessage(object o = null)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += _saveMessageBackground;
            worker.RunWorkerCompleted += _saveMessageCompleted;

            IsProgressbarVisible = true;
            IsProgressbarIndeterminate = true;
            worker.RunWorkerAsync();
        }

        private void _saveMessageBackground(object sender, DoWorkEventArgs e)
        {
            var hashsumHex = _calculateMessageJsonSha512();

            ProgressBarText = TranslationViewObject.MailProgressBarPreparingToSave;
            var mailAttachmentInfos = new List<MailAttachmentInfo>();
            foreach (var attachmentViewObject in MailAttachments)
            {
                string attachmentPath;
                if (_pathBySha512.ContainsKey(attachmentViewObject.Attachment.HashsumHex))
                {
                    attachmentPath = _pathBySha512[attachmentViewObject.Attachment.HashsumHex];
                }
                else
                {
                    attachmentPath = _storage.FindAttachmentPath(attachmentViewObject.Attachment.HashsumHex,
                                                                 SettingsManager.SettingsManager.GetHashsumEngine()
                                                                                .Calculate);
                    _pathBySha512[attachmentViewObject.Attachment.HashsumHex] = attachmentPath;
                }

                mailAttachmentInfos.Add(new MailAttachmentInfo(attachmentPath, attachmentViewObject.Attachment.HashsumHex));
            }

            ProgressBarText = TranslationViewObject.MailProgressBarSavingMessage;
            var messagePath = _storage.SaveMail(Mail, mailAttachmentInfos, SettingsManager.SettingsManager.GetHashsumEngine().Calculate);
            _savedMessagesSha512.Add(hashsumHex);
            MessageFilename = Path.GetFileName(messagePath);
            MailViewModelMode = MailViewModelMode.ViewExisting;

            if (RepositoryUpdatedHandlerNotify != null)
            {
                RepositoryUpdatedHandlerNotify.Invoke(
                    new MailViewReposytoryChangedResult(newMessagePaths: new[] {messagePath}));
            }
        }

        private void _saveMessageCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsProgressbarVisible = false;
            IsProgressbarIndeterminate = false;

            MessageBoxProvider.Show(TranslationViewObject.MailMessageBoxOperationFinished, TranslationViewObject.OverviewMessageBoxNotificationCaption);
        }

    }
}
