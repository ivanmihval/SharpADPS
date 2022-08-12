using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CoreADPS.Helpers;
using CoreADPS.Storage;
using CoreADPS.Storage.Models.ProgressModels;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Helpers.FolderBrowserDialogProvider;
using WPFSharpADPS.Helpers.MessageBoxProvider;
using WPFSharpADPS.Helpers.OpenFileDialogProvider;
using WPFSharpADPS.Helpers.SaveFileDialogProvider;
using WPFSharpADPS.Logging;
using WPFSharpADPS.Models;
using WPFSharpADPS.Models.Translation;
using WPFSharpADPS.SettingsManager;
using WPFSharpADPS.Views;

namespace WPFSharpADPS.ViewModels
{
    public class OverviewViewModel : PropertyChangedModel
    {
        private List<MailViewObject> _mails;
        private string[] _msgFileNames;
        private int _currentPageNumber;
        private int _mailsPerPage = SettingsManager.SettingsManager.Settings.MailsPerPage;

        private ulong _progressBarValue;
        private ulong _minProgressBarValue;
        private ulong _maxProgressBarValue = 100;

        private string _progressBarText;

        private bool _isWindowEnabled = true;

        private readonly bool _documentationFileExists;
        public const string DocumentationFilename = "documentation.txt";

        private Storage _storage;

        private TranslationViewObject _translationViewObject;

        public string[] PreviousRepositoryPaths { get { return SettingsManager.SettingsManager.Settings.PreviousRepositoryPaths; } }

        public IMessageBoxProvider MessageBoxProvider = new WinFormsMessageBoxProvider();
        public IFolderBrowserDialogProvider FolderBrowserDialogProvider = new WinFormsFolderBrowserDialogProvider();
        public ISaveFileDialogProvider SaveFileDialogProvider = new WinFormsSaveFileDialogProvider(); 
        public IOpenFileDialogProvider OpenFileDialogProvider = new WinFormsOpenFileDialogProvider();

        public ICommand ClickShowAllMailsButtonCommand { get; set; }
        public ICommand ClickCloseRepoButtonCommand { get; set; }

        public ICommand ClickNextPageButtonCommand { get; set; }
        public ICommand ClickPreviousPageButtonCommand { get; set; }

        public ICommand ClickIncreaseMailsPerPageButtonCommand { get; set; }
        public ICommand ClickDecreaseMailsPerPageButtonCommand { get; set; }

        public ICommand OpenMailCommand { get; set; }

        public ICommand NewRepositoryDialogCommand { get; set; }
        public ICommand OpenRepositoryDialogCommand { get; set; }
        public ICommand NewMailDialogCommand { get; set; }
        public ICommand CopyMailsToRepositoryDialogCommand { get; set; }
        public ICommand DeleteMailsDialogCommand { get; set; }
        public ICommand UnlistSelectedMails { get; set; }
        public ICommand LeaveOnlySelectedMails { get; set; }
        public ICommand OpenPreviousRepositoryDialogCommand { get; set; }
        public ICommand OpenApplyFiltersDialogCommand { get; set; }
        public ICommand ResetFiltersDialogCommand { get; set; }
        public ICommand SelectHashsumEngineType { get; set; }
        public ICommand SwitchLoggingStateCommand { get; set; }
        public ICommand DumpLogsCommand { get; set; }
        public ICommand CleanLogsCommand { get; set; }
        public ICommand ChooseTranslationCommand { get; set; }
        public ICommand AddExternalTranslationCopyCommand { get; set; }
        public ICommand AddExistingTranslationCommand { get; set; }
        public ICommand RemoveExternalTranslationsCommand { get; set; }

        public ICommand OpenDocumentationFolder { get; set; }

        public Logger Logger = new Logger("OverviewViewModel");

        private void OnFilterProgressChanged(FilterMailsProgressData filterMailsProgressData)
        {
            MinProgressBarValue = 0;
            MaxProgressBarValue = (ulong) filterMailsProgressData.TotalMailsNumber;
            ProgressBarValue = (ulong) (filterMailsProgressData.CurrentIndex + 1);
            ProgressBarText = _translationViewObject.OverviewProgressBarFilteringMessages;

            if (_maxProgressBarValue == _progressBarValue)
            {
                ProgressBarValue = 0;
                ProgressBarText = null;
            }
        }

        private void OnCopyProgressChanged(CopyMailsProgressData copyMailsProgressData)
        {
            MinProgressBarValue = 0;
            // ReSharper disable PossibleInvalidOperationException
            MaxProgressBarValue = copyMailsProgressData.Stage == CopyMailsStage.Estimation
                                      ? (ulong) copyMailsProgressData.EstimationProgressData.Value.TotalMailsNumber
                                      : copyMailsProgressData.CopyingProgressData.Value.TotalFilesSizeBytes;
            ProgressBarValue = copyMailsProgressData.Stage == CopyMailsStage.Estimation
                                      ? (ulong)copyMailsProgressData.EstimationProgressData.Value.CurrentIndex
                                      : copyMailsProgressData.CopyingProgressData.Value.CopiedBytes;
            // ReSharper restore PossibleInvalidOperationException
            ProgressBarText = copyMailsProgressData.Stage == CopyMailsStage.Estimation
                                      ? _translationViewObject.OverviewProgressBarCopyEstimation
                                      : _translationViewObject.OverviewProgressBarCopying;

            if (_maxProgressBarValue == _progressBarValue)
            {
                ProgressBarValue = 0;
                ProgressBarText = null;
            }
        }

        private void OnDeleteEstimationProgressChanged(DeleteMailsEstimationProgressData deleteMailsProgressData)
        {
            MinProgressBarValue = 0;

            MaxProgressBarValue = (ulong) deleteMailsProgressData.TotalMailsNumber;
            ProgressBarValue = (ulong) deleteMailsProgressData.CurrentIndex;
            ProgressBarText = deleteMailsProgressData.Stage == DeleteMailsEstimationStage.ScanningTargetFiles
                                  ? _translationViewObject.OverviewProgressBarScanningTargetFiles
                                  : _translationViewObject.OverviewProgressBarScanningAllFiles;

            if (deleteMailsProgressData.Stage == DeleteMailsEstimationStage.ScanningAllFiles &&
                (deleteMailsProgressData.CurrentIndex + 1 == deleteMailsProgressData.TotalMailsNumber))
            {
                ProgressBarValue = 0;
                ProgressBarText = null;
            }
        }

        private void OnMailViewClosed(MailViewReposytoryChangedResult mailViewReposytoryChangedResult)
        {
            if (mailViewReposytoryChangedResult.DeletedMessagePaths.Length +
                mailViewReposytoryChangedResult.NewMessagePaths.Length > 0)
            {
                _msgFileNames =
                    _msgFileNames.Union(mailViewReposytoryChangedResult.NewMessagePaths.Select(Path.GetFileName))
                                 .Except(mailViewReposytoryChangedResult.DeletedMessagePaths.Select(Path.GetFileName))
                                 .Distinct()
                                 .ToArray();

                _renderPage();
            }
        }

        private void _loadRepo(string path)
        {
            Storage = new Storage(path);
            Storage.FilterMailsHandlerNotify += OnFilterProgressChanged;
            Storage.CopyMailsHandlerNotify += OnCopyProgressChanged;
            Storage.DeleteMailsEstimationHandlerNotify += OnDeleteEstimationProgressChanged;
            Storage.Logger = IsLoggingActive ? null : new Logger("Storage");

            MsgFileNames = Storage.GetMsgPaths();
            CurrentPageNumber = 1;
            OnPropertyChanged("MaxPageNumber");
        }

        private void _clickLoadRepoButton(object o = null)
        {
            string targetFolder;
            if (FolderBrowserDialogProvider.ChooseFolder(out targetFolder))
            {
                _loadRepo(targetFolder);
                SettingsManager.SettingsManager.Settings.AddRepository(targetFolder);
            }
        }

        private void _clickLoadPreviousRepository(object arg)
        {
            var path = (string) arg;
            _loadRepo(path);
            SettingsManager.SettingsManager.Settings.AddRepository(path);
            OnPropertyChanged("PreviousRepositoryPaths");
        }

        private void _clickNewRepository(object arg)
        {
            string targetFolder;
            if (FolderBrowserDialogProvider.ChooseFolder(out targetFolder))
            {
                if (Directory.EnumerateFileSystemEntries(targetFolder).Any())
                {
                    MessageBoxProvider.Show(_translationViewObject.OverviewMessageBoxDirectoryNotEmpty,
                                            _translationViewObject.OverviewMessageBoxErrorCaption);
                    return;
                }

                Storage.CreateNewRepository(targetFolder);

                _loadRepo(targetFolder);
                SettingsManager.SettingsManager.Settings.AddRepository(targetFolder);
                OnPropertyChanged("PreviousRepositoryPaths");
            }
        }

        private bool _canClickLoadPreviousRepository(object arg)
        {
            var path = (string) arg;
            return Storage == null || Storage.RootDirPath != path;
        }

        private void _renderPage()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += _backgroundRenderPage;
            worker.RunWorkerCompleted += _renderPageRunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void _renderPageRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsWindowEnabled = true;
        }

        private void _backgroundRenderPage(object sender, DoWorkEventArgs e)
        {
            IsWindowEnabled = false;
            var skipMailsNumber = (_currentPageNumber - 1) * MailsPerPage;
            if (_msgFileNames == null || Storage == null)
            {
                return;
            }
            Mails =
                new List<MailViewObject>(
                    Storage.FilteredMailResults(null, _msgFileNames.Skip(skipMailsNumber).Take(MailsPerPage).ToArray())
                           .Select(fmr => new MailViewObject(fmr.Mail, fmr.MailPath, _translationViewObject)));
        }

        public string Title
        {
            get
            {
                const string fixedPart = "ADPS - ";
                var flexiblePart = _storage == null ? _translationViewObject.OverviewNoRepoLoadedTitle : _storage.RootDirPath;
                return fixedPart + flexiblePart;
            }
        }

        public Storage Storage
        {
            get { return _storage; }
            set
            {
                _storage = value;
                Mails = null;
                OnPropertyChanged("Storage");
                OnPropertyChanged("Title");
            }
        }

        public List<MailViewObject> Mails
        {
            get { return _mails; }
            set { _mails = value; OnPropertyChanged("Mails"); }
        }

        public string[] MsgFileNames
        {
            get { return _msgFileNames; }
            set { _msgFileNames = value; OnPropertyChanged("MsgFileNames"); }
        }

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set
            {
                _currentPageNumber = Math.Max(1, Math.Min(value, MaxPageNumber));
                _renderPage();
                OnPropertyChanged("CurrentPageNumber");
            }
        }

        public int MaxPageNumber
        {
            get
            {
                if (_msgFileNames == null || _msgFileNames.Length == 0)
                {
                    return 1;
                }
                return (int) Math.Ceiling((double) _msgFileNames.Length/MailsPerPage);
            }
        }

        public int MailsPerPage
        {
            get { return _mailsPerPage; }
            set
            {
                _mailsPerPage = value;
                _renderPage();
                SettingsManager.SettingsManager.Settings.MailsPerPage = value;
                OnPropertyChanged("MailsPerPage");
                OnPropertyChanged("MaxPageNumber");
            }
        }

        public ulong ProgressBarValue
        {
            get { return _progressBarValue; }
            set { _progressBarValue = value; OnPropertyChanged("ProgressBarValue"); }
        }

        public ulong MinProgressBarValue
        {
            get { return _minProgressBarValue; }
            set { _minProgressBarValue = value; OnPropertyChanged("MinProgressBarValue"); }
        }

        public ulong MaxProgressBarValue
        {
            get { return _maxProgressBarValue; }
            set { _maxProgressBarValue = value; OnPropertyChanged("MaxProgressBarValue"); }
        }

        public string ProgressBarText
        {
            get { return _progressBarText; }
            set { _progressBarText = value; OnPropertyChanged("ProgressBarText"); }
        }

        public bool IsWindowEnabled
        {
            get { return _isWindowEnabled; }
            set { _isWindowEnabled = value; OnPropertyChanged("IsWindowEnabled"); }
        }

        public List<HashsumEngineViewObject> HashsumEngineViewObjects
        {
            get
            {
                return
                    new List<HashsumEngineViewObject>(
                        Helpers.HashsumEngine.HashsumEngineManager.GetHashsumEngines()
                               .Select(HashsumEngineViewObject.FromIHashsumEngine)
                               .ToList());
            }
        }

        public List<TranslationMenuItem> TranslationMenuItems
        {
            get { return TranslationMenuItem.GetAllTranslationMenuItems(); }
        } 

        public bool IsLoggingActive
        {
            get { return GlobalLoggingStorage.State == GlobalLoggingStorageState.Active; }
            set
            {
                GlobalLoggingStorage.State = value
                                                 ? GlobalLoggingStorageState.Active
                                                 : GlobalLoggingStorageState.Inactive;
                OnPropertyChanged("IsLoggingActive");
            }
        }

        public TranslationViewObject TranslationViewObject
        {
            get { return _translationViewObject; }
            set
            {
                _translationViewObject = value;
                OnPropertyChanged("TranslationViewObject");
            }
        }

        private int _selectedCount()
        {
            if (Mails == null)
            {
                return 0;
            }
            return Mails.Count(m => m.IsSelected);
        }

        private void _openMail(object o = null)
        {
            if (Mails == null)
            {
                return;
            }

            var selectedMails = Mails.Where(m => m.IsSelected).ToList();
            if (selectedMails.Count != 1)
            {
                return;
            }

            var selectedMail = selectedMails.First();
            var mailViewModel = new MailViewModel(selectedMail.MsgPath, Storage.RootDirPath);
            mailViewModel.RepositoryUpdatedHandlerNotify += OnMailViewClosed;
            var window = new MailView(mailViewModel);
            window.ShowDialog();
        }

        private void _newMail(object o = null)
        {
            var mailViewModel = new MailViewModel(storagePath: Storage.RootDirPath);
            mailViewModel.RepositoryUpdatedHandlerNotify += OnMailViewClosed;
            var window = new MailView(mailViewModel);
            window.ShowDialog();
        }

        private void _copyMailsToRepositoryBackground(object sender, DoWorkEventArgs e)
        {
            var targetRepoPath = (string) e.Argument;
            var engine = SettingsManager.SettingsManager.GetHashsumEngine();
            try
            {
                var estimationResult = Storage.GetCopyMailsEstimationResult(_msgFileNames, engine.Calculate);
                Storage.CopyMails(estimationResult, targetRepoPath, engine.Calculate);
            }
            catch (Exception exc)
            {
                MessageBoxProvider.Show(String.Format("Could not copy mails: {0}", exc), "Error");
                Logger.Write(LoggingLevel.Error, String.Format("Could not copy mails: {0}", exc));
            }
        }

        private void _copyMailsToRepository(object o = null)
        {
            string targetFolder;
            if (FolderBrowserDialogProvider.ChooseFolder(out targetFolder))
            {
                if (!Storage.RepositoryIsValid(targetFolder))
                {
                    MessageBoxProvider.Show(_translationViewObject.OverviewMessageBoxInvalidAdpsDirectory,
                                            _translationViewObject.OverviewMessageBoxErrorCaption);
                    return;
                }

                var worker = new BackgroundWorker();
                worker.DoWork += _copyMailsToRepositoryBackground;
                worker.RunWorkerCompleted += _renderPageRunWorkerCompleted;

                IsWindowEnabled = false;
                worker.RunWorkerAsync(argument: targetFolder);
            }
        }

        private void _deleteMailsBackground(object sender, DoWorkEventArgs e)
        {
            var engine = SettingsManager.SettingsManager.GetHashsumEngine();
            var attachmentPathsToDelete = Storage.GetAttachmentPathsForDelete(_msgFileNames, engine.Calculate);
            var filePathsToDelete = _msgFileNames.Select(
                fileName => Path.Combine(_storage.RootDirPath, Storage.MessagesFolder, fileName))
                                                 .Union(attachmentPathsToDelete).ToArray();

            MaxProgressBarValue = (ulong) filePathsToDelete.Length;
            ProgressBarText = _translationViewObject.OverviewProgressBarDeleteFiles;

            ulong idx = 0;
            foreach (var filePathToDelete in filePathsToDelete)
            {
                File.Delete(filePathToDelete);
                ProgressBarValue = ++idx;
            }

            foreach (var renameMappingItem in Storage.GetAttachmentRenameMapping())
            {
                File.Move(renameMappingItem.Item1, renameMappingItem.Item2);
            }

            ProgressBarValue = 0;
            ProgressBarText = null;

            MsgFileNames = Storage.GetMsgPaths();
            CurrentPageNumber = 1;
            OnPropertyChanged("MaxPageNumber");
            _backgroundRenderPage(null, null);

            MessageBoxProvider.Show(_translationViewObject.OverviewMessageBoxMailsDeletedNotification,
                                    _translationViewObject.OverviewMessageBoxNotificationCaption);
        }

        private bool _hasFilteredMails(object o = null)
        {
            return _storage != null && _msgFileNames != null && _msgFileNames.Length != 0;
        }

        private void _deleteMails(object o = null)
        {
            var confirmResult =
                MessageBoxProvider.Confirm(
                    String.Format(_translationViewObject.OverviewMessageBoxDeleteFileConfirmationTemplate,
                                  _msgFileNames.Length), _translationViewObject.OverviewMessageBoxConfirmationCaption);
            if (confirmResult == MessageBoxConfirmResult.No)
            {
                return;
            }

            var worker = new BackgroundWorker();
            worker.DoWork += _deleteMailsBackground;
            worker.RunWorkerCompleted += _renderPageRunWorkerCompleted;

            IsWindowEnabled = false;
            worker.RunWorkerAsync();
        }

        private void _openApplyFiltersBackground(object sender, DoWorkEventArgs e)
        {
            var viewModel = (MailsFilterViewModel) e.Argument;
            IsWindowEnabled = false;

            switch (viewModel.Result.Status)
            {
                case MailsFilterDialogResultStatus.NewSearch:
                    MsgFileNames = viewModel.Result.Filters.Any()
                                   ? Storage.FilteredMailResults(viewModel.Result.Filters)
                                            .Select(fmr => Path.GetFileName(fmr.MailPath))
                                            .ToArray()
                                   : Storage.GetMsgPaths();
                    CurrentPageNumber = 1;
                    break;
                case MailsFilterDialogResultStatus.UniteResults:
                    MsgFileNames = viewModel.Result.Filters.Any()
                                       ? MsgFileNames.Union(
                                           Storage.FilteredMailResults(viewModel.Result.Filters)
                                                  .Select(fmr => Path.GetFileName(fmr.MailPath)))
                                                  .ToArray()
                                       : Storage.GetMsgPaths();
                    break;
                case MailsFilterDialogResultStatus.RefineCurrentSearch:
                    if (viewModel.Result.Filters.Any())
                    {
                        MsgFileNames = Storage.FilteredMailResults(viewModel.Result.Filters, MsgFileNames)
                                              .Select(fmr => Path.GetFileName(fmr.MailPath))
                                              .ToArray();
                        CurrentPageNumber = 1;
                    }
                    break;
            }

            OnPropertyChanged("MaxPageNumber");
        }

        private void _openApplyFiltersDialog(object o = null)
        {
            var viewModel = new MailsFilterViewModel();
            var window = new MailsFilterView(viewModel);
            window.ShowDialog();

            if (viewModel.Result != null && viewModel.Result.Status != MailsFilterDialogResultStatus.CancelClicked)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += _openApplyFiltersBackground;
                worker.RunWorkerCompleted += _renderPageRunWorkerCompleted;
                worker.RunWorkerAsync(argument: viewModel);
            }
        }

        private void _resetFilters(object o = null)
        {
            MsgFileNames = Storage.GetMsgPaths();
            CurrentPageNumber = 1;
            OnPropertyChanged("MaxPageNumber");
        }

        private void _selectHashsumEngine(object o)
        {
            var hashsumEngineViewObject = (HashsumEngineViewObject) o;
            if (hashsumEngineViewObject.IsSelected)
            {
                return;
            }

            SettingsManager.SettingsManager.Settings.HashsumEngineType =
                hashsumEngineViewObject.HashsumEngine.GetEngineType();
            OnPropertyChanged("HashsumEngineViewObjects");
        }

        private bool _canSelectHashsumEngine(object o)
        {
            if (o == null)
            {
                return false;
            }
            var hashsumEngineViewObject = (HashsumEngineViewObject) o;
            return !hashsumEngineViewObject.IsSelected;
        }

        private bool _hasSelectedMails(object o = null)
        {
            return _selectedCount() > 0;
        }

        private void _unlistSelectedMails(object o = null)
        {
            var selectedMailPaths =
                new HashSet<string>(Mails.Where(m => m.IsSelected).Select(m => Path.GetFileName(m.MsgPath)));
            MsgFileNames = MsgFileNames.Where(m => !selectedMailPaths.Contains(m)).ToArray();
            OnPropertyChanged("MaxPageNumber");
            if (selectedMailPaths.Count == Mails.Count)
            {
                CurrentPageNumber = Math.Max(1, CurrentPageNumber - 1);
            }
            else
            {
                _renderPage();
            }
        }

        private void _leaveOnlySelectedMails(object o = null)
        {
            MsgFileNames = Mails.Where(m => m.IsSelected).Select(m => Path.GetFileName(m.MsgPath)).ToArray();
            OnPropertyChanged("MaxPageNumber");
            CurrentPageNumber = 1;
        }

        private void _switchLoggingState(object o = null)
        {
            _storage.Logger = IsLoggingActive ? null : new Logger("Storage");
            IsLoggingActive = !IsLoggingActive;
        }

        private bool _canDumpLogs(object o = null)
        {
            return GlobalLoggingStorage.Entries.Any();
        }

        private void _dumpLogs(object o = null)
        {
            string targetFilePath;
            if (SaveFileDialogProvider.ChooseFile(out targetFilePath))
            {
                File.WriteAllLines(targetFilePath, GlobalLoggingStorage.Entries);
                MessageBoxProvider.Show("The logs have been saved to the file.", "Notification");
            }
        }

        private void _cleanLogs(object o = null)
        {
            GlobalLoggingStorage.CleanEntries();
        }

        private bool _canChooseTranslation(object o)
        {
            var translationItem = o as TranslationMenuItem;
            if (translationItem == null)
            {
                return false;
            }

            return !translationItem.IsSelected;
        }

        private void _chooseTranslation(object o)
        {
            var translationItem = o as TranslationMenuItem;
            if (translationItem == null)
            {
                return;
            }

            var isTranslationValid = translationItem.Mode == TranslationLoadingMode.Embedded;
            if (translationItem.Mode == TranslationLoadingMode.ExternalFile)
            {
                var path = TranslationViewObject.FindTranslationPathByKey(translationItem.Key);
                if (path != null)
                {
                    try
                    {
                        TranslationViewObject.LoadTranslationModel(path);
                        isTranslationValid = true;
                    }
// ReSharper disable EmptyGeneralCatchClause
                    catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }

                if (!isTranslationValid)
                {
                    var dialogResult = MessageBoxProvider.Confirm(
                        "The translation is not valid, delete from the list?", "Confirmation");
                    if (dialogResult == MessageBoxConfirmResult.Yes)
                    {
                        SettingsManager.SettingsManager.Settings.RemoveExternalTranslation(translationItem.Key);
                        OnPropertyChanged("TranslationMenuItems");
                    }
                }
            }

            if (isTranslationValid)
            {
                SettingsManager.SettingsManager.Settings.TranslationKey = translationItem.Key;
                SettingsManager.SettingsManager.Settings.TranslationLoadingMode = translationItem.Mode;
                TranslationViewObject = TranslationViewObject.LoadFromSettings();
            }
        }

        public void _addExternalTranslationCopy(object o = null)
        {
            string outputPath;
            var isSelected = SaveFileDialogProvider.ChooseFile(out outputPath);
            if (isSelected)
            {
                var newTranslation = TranslationViewObject.SaveCopy(outputPath, TranslationViewObject.TranslationModel);
                SettingsManager.SettingsManager.Settings.TranslationKey = newTranslation.Key;
                SettingsManager.SettingsManager.Settings.TranslationLoadingMode = TranslationLoadingMode.ExternalFile;
                SettingsManager.SettingsManager.Settings.AddExternalTranslation(newTranslation.Key, newTranslation.Title, outputPath);
                OnPropertyChanged("TranslationMenuItems");
                TranslationViewObject = TranslationViewObject.LoadFromSettings();
            }
        }

        private bool _canRemoveExternalTranslations(object o = null)
        {
            return TranslationMenuItems.Any(tmi => tmi.Mode == TranslationLoadingMode.ExternalFile);
        }

        private void _removeExternalTranslations(object o = null)
        {
            SettingsManager.SettingsManager.Settings.ExternalTranslationKeys = new string[0];
            SettingsManager.SettingsManager.Settings.ExternalTranslationTitles = new string[0];
            SettingsManager.SettingsManager.Settings.ExternalTranslationPaths = new string[0];
            OnPropertyChanged("TranslationMenuItems");
            if (SettingsManager.SettingsManager.Settings.TranslationLoadingMode == TranslationLoadingMode.ExternalFile)
            {
                TranslationViewObject = TranslationViewObject.LoadFromSettings();
            }
        }

        private void _addExistingTranslation(object o = null)
        {
            string inputPath;
            var isSelected = OpenFileDialogProvider.ChooseFile(out inputPath);
            if (isSelected)
            {
                TranslationModel newTranslation;
                try
                {
                    newTranslation = TranslationViewObject.LoadTranslationModel(inputPath);
                }
                catch (Exception)
                {
                    MessageBoxProvider.Show("Could not load the translation.", TranslationViewObject.OverviewMessageBoxErrorCaption);
                    return;
                }
                
                SettingsManager.SettingsManager.Settings.TranslationKey = newTranslation.Key;
                SettingsManager.SettingsManager.Settings.TranslationLoadingMode = TranslationLoadingMode.ExternalFile;
                SettingsManager.SettingsManager.Settings.AddExternalTranslation(newTranslation.Key, newTranslation.Title, inputPath);
                OnPropertyChanged("TranslationMenuItems");
                TranslationViewObject = TranslationViewObject.LoadFromSettings();
            }
        }

        private string _getDocumentationPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentationFilename);
        }

        private bool _canOpenDocumentationFolder(object o = null)
        {
            return _documentationFileExists;
        }

        private void _openDocumentationFolder(object o = null)
        {
            var path = _getDocumentationPath();
            WindowsExplorer.OpenFolderAndSelectItem(path);
        }

        public OverviewViewModel()
        {
            ClickShowAllMailsButtonCommand = new CommandHandler(_resetFilters, _ => _storage != null);
            ClickCloseRepoButtonCommand = new CommandHandler(_ => { Storage = null; }, _ => _storage != null);

            ClickNextPageButtonCommand = new CommandHandler(_ => { CurrentPageNumber++; },
                                                            _ =>
                                                            _storage != null && _mails != null &&
                                                            CurrentPageNumber < MaxPageNumber);
            ClickPreviousPageButtonCommand = new CommandHandler(_ => { CurrentPageNumber--; },
                                                                _ => _storage != null && CurrentPageNumber > 1);

            ClickIncreaseMailsPerPageButtonCommand = new CommandHandler(_ => { MailsPerPage++; }, _ => true);
            ClickDecreaseMailsPerPageButtonCommand = new CommandHandler(_ => { MailsPerPage--; }, _ => MailsPerPage > 1);

            OpenMailCommand = new CommandHandler(_openMail, _ => _selectedCount() == 1);

            NewRepositoryDialogCommand = new CommandHandler(_clickNewRepository, _ => true);
            OpenRepositoryDialogCommand = new CommandHandler(_clickLoadRepoButton, _ => true);
            NewMailDialogCommand = new CommandHandler(_newMail, _ => _storage != null);
            CopyMailsToRepositoryDialogCommand = new CommandHandler(_copyMailsToRepository, _hasFilteredMails);
            DeleteMailsDialogCommand = new CommandHandler(_deleteMails, _hasFilteredMails);
            UnlistSelectedMails = new CommandHandler(_unlistSelectedMails, _hasSelectedMails);
            LeaveOnlySelectedMails = new CommandHandler(_leaveOnlySelectedMails, _hasSelectedMails);
            OpenPreviousRepositoryDialogCommand = new CommandHandler(_clickLoadPreviousRepository,
                                                                     _canClickLoadPreviousRepository);
            OpenApplyFiltersDialogCommand = new CommandHandler(_openApplyFiltersDialog,
                                                               _ => _storage != null && _mails != null);
            ResetFiltersDialogCommand = new CommandHandler(_resetFilters, _ => _storage != null);

            SelectHashsumEngineType = new CommandHandler(_selectHashsumEngine, _canSelectHashsumEngine);
            SwitchLoggingStateCommand = new CommandHandler(_switchLoggingState, _ => true);
            DumpLogsCommand = new CommandHandler(_dumpLogs, _canDumpLogs);
            CleanLogsCommand = new CommandHandler(_cleanLogs, _ => true);
            ChooseTranslationCommand = new CommandHandler(_chooseTranslation, _canChooseTranslation);
            AddExternalTranslationCopyCommand = new CommandHandler(_addExternalTranslationCopy, _ => true);
            RemoveExternalTranslationsCommand = new CommandHandler(_removeExternalTranslations, _canRemoveExternalTranslations);
            AddExistingTranslationCommand = new CommandHandler(_addExistingTranslation, _ => true);

            _documentationFileExists = File.Exists(_getDocumentationPath());
            OpenDocumentationFolder = new CommandHandler(_openDocumentationFolder, _canOpenDocumentationFolder);

            TranslationViewObject = TranslationViewObject.LoadFromSettings();

            if (SettingsManager.SettingsManager.Settings != null &&
                SettingsManager.SettingsManager.Settings.PreviousRepositoryPaths != null &&
                SettingsManager.SettingsManager.Settings.PreviousRepositoryPaths.Any())
            {
                var repoPath = SettingsManager.SettingsManager.Settings.PreviousRepositoryPaths[0];
                try
                {
                    _loadRepo(repoPath);
                }
                catch (Exception e)
                {
                    MessageBoxProvider.Show(String.Format("Error during loading repository: {0}", e), "Error");
                    SettingsManager.SettingsManager.Settings.RemoveRepository(repoPath);
                }
            }
        }
    }
}
