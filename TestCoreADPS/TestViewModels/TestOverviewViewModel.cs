using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreADPS.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCoreADPS.Helpers;
using TestCoreADPS.TestHelpers;
using TestCoreADPS.TestViewModels.Providers;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Helpers.HashsumEngine;
using WPFSharpADPS.Logging;
using WPFSharpADPS.SettingsManager;
using WPFSharpADPS.ViewModels;

namespace TestCoreADPS.TestViewModels
{
    public class OverviewViewTestModel : OverviewViewModel
    {
        public readonly List<string> OnPropertyCalledArgs = new List<string>();

        private void _onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyCalledArgs.Add(e.PropertyName);
        }

        public OverviewViewTestModel()
        {
            PropertyChanged += _onPropertyChanged;
            MessageBoxProvider = new MockedMessageBoxProvider();
            FolderBrowserDialogProvider = new MockedFolderBrowserDialogProvider();
            MailsPerPage = 5;
        }

        public OverviewViewTestModel(string storagePath, int mailsPerPage = 5) : this()
        {
            MailsPerPage = mailsPerPage;

            ((MockedFolderBrowserDialogProvider)FolderBrowserDialogProvider).OutputPath = storagePath;
            OpenRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(() => Mails != null);
        }

        private static void _initSettings()
        {
            SettingsManager.Settings.HashsumEngineType = HashsumEngineType.DotNet;
            SettingsManager.Settings.PreviousRepositoryPaths = new string[] { };
        }

        public static OverviewViewTestModel Default()
        {
            _initSettings();
            return new OverviewViewTestModel();
        }

        public static OverviewViewTestModel DefaultByRepoPath(string storagePath, int mailsPerPage = 5)
        {
            _initSettings();
            return new OverviewViewTestModel(storagePath, mailsPerPage);
        }
    }

    [TestClass]
    public class TestOverviewViewModel
    {
        [TestMethod]
        public void TestCreateNewRepo()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestCreateNewRepo", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);

            Directory.CreateDirectory(repoFolderPath);

            var viewModel = OverviewViewTestModel.Default();
            ((MockedFolderBrowserDialogProvider) viewModel.FolderBrowserDialogProvider).OutputPath = repoFolderPath;
            viewModel.NewRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => viewModel.Storage != null && viewModel.Storage.RootDirPath == repoFolderPath);

            var directories = new List<string>(Directory.GetDirectories(repoFolderPath).Select(Path.GetFileName).OrderBy(x => x));
            var expectedFolders = new List<string> {"adps_attachments", "adps_messages"};
            Assert.IsTrue(directories.SequenceEqual(expectedFolders));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestOpenRepo()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestOpenRepo", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            var mails = GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.Default();
            Assert.AreEqual(0, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            ((MockedFolderBrowserDialogProvider) viewModel.FolderBrowserDialogProvider).OutputPath = repoFolderPath;
            viewModel.OpenRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(() => (viewModel.Mails != null && viewModel.Mails.Count > 0));

            Assert.IsFalse(viewModel.Mails == null);
            Assert.AreEqual(mails.Count, viewModel.Mails.Count);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestUnhandledException()
        {
            var viewModel = OverviewViewTestModel.Default();
            viewModel.IsLoggingActive = true;
            var commandHandler = new CommandHandler(_ => { throw new NotImplementedException(); }, _ => true)
                {
                    MessageBoxProvider = new MockedMessageBoxProvider()
                };
            viewModel.OpenRepositoryDialogCommand = commandHandler;
            viewModel.OpenRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(() => (GlobalLoggingStorage.Entries.Count > 0));
            var entry = GlobalLoggingStorage.Entries.First(x => x.StartsWith("CommandHandler"));
            Assert.IsTrue(entry.Contains("System.NotImplementedException: The method or operation is not implemented"));

            var mockedMessageBox = (MockedMessageBoxProvider) commandHandler.MessageBoxProvider;
            Assert.AreEqual(1, mockedMessageBox.Calls.Count);
            Assert.AreEqual("Error", mockedMessageBox.Calls[0].Item2);
            Assert.IsTrue(mockedMessageBox.Calls[0].Item1.Contains("System.NotImplementedException"));
        }

        [TestMethod]
        public void TestSaveFilteredMails()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestSaveFilteredMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var targetRepoFolderPath = Path.Combine(repoFolderPath, "target_repo");
            var targetRepoMessagesPath = Path.Combine(targetRepoFolderPath, Storage.MessagesFolder);
            var targetRepoAttachmentsPath = Path.Combine(targetRepoFolderPath, Storage.AttachmentsFolder);
            Directory.CreateDirectory(targetRepoMessagesPath);
            Directory.CreateDirectory(targetRepoAttachmentsPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            ((MockedFolderBrowserDialogProvider)viewModel.FolderBrowserDialogProvider).OutputPath = targetRepoFolderPath;
            viewModel.CopyMailsToRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(3, Directory.GetFiles(targetRepoAttachmentsPath).Count());
            Assert.AreEqual(2, Directory.GetFiles(targetRepoMessagesPath).Count());

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestSaveFilteredMailsHandledException()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestSaveFilteredMailsHandledException", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            // delete attachments in order to corrupt the repository
            foreach (var attachmentPath in Directory.GetFiles(Path.Combine(repoFolderPath, Storage.AttachmentsFolder)))
            {
                File.Delete(attachmentPath);
            }

            var targetRepoFolderPath = Path.Combine(repoFolderPath, "target_repo");
            var targetRepoMessagesPath = Path.Combine(targetRepoFolderPath, Storage.MessagesFolder);
            var targetRepoAttachmentsPath = Path.Combine(targetRepoFolderPath, Storage.AttachmentsFolder);
            Directory.CreateDirectory(targetRepoMessagesPath);
            Directory.CreateDirectory(targetRepoAttachmentsPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            viewModel.IsLoggingActive = true;
            var commandHandler = (CommandHandler) viewModel.CopyMailsToRepositoryDialogCommand;
            commandHandler.MessageBoxProvider = new MockedMessageBoxProvider();

            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            ((MockedFolderBrowserDialogProvider)viewModel.FolderBrowserDialogProvider).OutputPath = targetRepoFolderPath;
            viewModel.CopyMailsToRepositoryDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
               () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(0, Directory.GetFiles(targetRepoAttachmentsPath).Count());
            Assert.AreEqual(0, Directory.GetFiles(targetRepoMessagesPath).Count());

            Assert.IsTrue(GlobalLoggingStorage.Entries.Count > 0);
            Assert.AreEqual(1, GlobalLoggingStorage.Entries.Count(entry => entry.StartsWith("OverviewViewModel")));

            var errorEntry = GlobalLoggingStorage.Entries.First(entry => entry.StartsWith("OverviewViewModel"));
            Assert.IsTrue(errorEntry.Contains("Could not find attachment for sha512"));

            Assert.AreEqual(1, ((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls.Count);
            Assert.IsTrue(((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls[0].Item1.Contains("Could not find attachment for sha512"));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestDeleteFilteredMails()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestDeleteFilteredMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            Assert.AreEqual(3, Directory.GetFiles(Path.Combine(repoFolderPath, Storage.AttachmentsFolder)).Count());
            Assert.AreEqual(2, Directory.GetFiles(Path.Combine(repoFolderPath, Storage.MessagesFolder)).Count());
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            viewModel.DeleteMailsDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(0, Directory.GetFiles(Path.Combine(repoFolderPath, Storage.AttachmentsFolder)).Count());
            Assert.AreEqual(0, Directory.GetFiles(Path.Combine(repoFolderPath, Storage.MessagesFolder)).Count());

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestUnlistSelectedMails()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestUnlistSelectedMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            Assert.IsFalse(viewModel.UnlistSelectedMails.CanExecute(null));
            Assert.IsNotNull(viewModel.Mails, "viewModel.Mails != null");
            viewModel.Mails.ElementAt(0).IsSelected = true;
            Assert.IsTrue(viewModel.UnlistSelectedMails.CanExecute(null));
            viewModel.UnlistSelectedMails.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(viewModel.Mails.Count, 1);
            Assert.AreEqual("bee12b5bd6.json", Path.GetFileName(viewModel.Mails.ElementAt(0).MsgPath));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestLeaveOnlySelectedMails()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestLeaveOnlySelectedMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            Assert.IsFalse(viewModel.LeaveOnlySelectedMails.CanExecute(null));
            Assert.IsNotNull(viewModel.Mails, "viewModel.Mails != null");
            viewModel.Mails.ElementAt(0).IsSelected = true;
            Assert.IsTrue(viewModel.LeaveOnlySelectedMails.CanExecute(null));
            viewModel.LeaveOnlySelectedMails.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(viewModel.Mails.Count, 1);
            Assert.AreEqual("0d948fdc77.json", Path.GetFileName(viewModel.Mails.ElementAt(0).MsgPath));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestResetFiltersDialogCommand()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestResetFiltersDialogCommand", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            Assert.IsFalse(viewModel.UnlistSelectedMails.CanExecute(null));
            Assert.IsNotNull(viewModel.Mails, "viewModel.Mails != null");
            viewModel.Mails.ElementAt(0).IsSelected = true;
            viewModel.Mails.ElementAt(1).IsSelected = true;
            Assert.IsTrue(viewModel.UnlistSelectedMails.CanExecute(null));
            viewModel.UnlistSelectedMails.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(viewModel.Mails.Count, 0);
            isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            viewModel.ResetFiltersDialogCommand.Execute(null);

            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));

            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestClickCloseRepoButtonCommand()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestClickCloseRepoButtonCommand", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath);
            Assert.IsNotNull(viewModel.Storage);

            viewModel.ClickCloseRepoButtonCommand.Execute(null);
            WaitUntil.WaitUntilFuncIsTrue(() => viewModel.Storage == null);
            Assert.IsNull(viewModel.Mails);
            Assert.IsFalse(viewModel.ClickCloseRepoButtonCommand.CanExecute(null));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestPagination()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestPagination", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            GenerateTestRepo.Generate(repoFolderPath);

            var viewModel = OverviewViewTestModel.DefaultByRepoPath(repoFolderPath, 2);
            Assert.AreEqual(2, viewModel.Mails == null ? 0 : viewModel.Mails.Count);
            Assert.AreEqual(2, viewModel.MailsPerPage);
            var isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");
            
            // Click "-"
            viewModel.ClickDecreaseMailsPerPageButtonCommand.Execute(null);
            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));
            Assert.IsFalse(viewModel.ClickDecreaseMailsPerPageButtonCommand.CanExecute(null));
            Assert.IsFalse(viewModel.ClickPreviousPageButtonCommand.CanExecute(null));
            Assert.IsNotNull(viewModel.Mails);
            Assert.AreEqual(1, viewModel.Mails.Count);
            Assert.AreEqual("0d948fdc77.json", Path.GetFileName(viewModel.Mails.ElementAt(0).MsgPath));
            isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            // Click ">"
            viewModel.ClickNextPageButtonCommand.Execute(null);
            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 2));
            Assert.IsFalse(viewModel.ClickNextPageButtonCommand.CanExecute(null));
            Assert.IsNotNull(viewModel.Mails);
            Assert.AreEqual(1, viewModel.Mails.Count);
            Assert.AreEqual("bee12b5bd6.json", Path.GetFileName(viewModel.Mails.ElementAt(0).MsgPath));
            isWindowsEnabledCount = viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled");

            // Click "<" and "+"
            viewModel.ClickPreviousPageButtonCommand.Execute(null);
            viewModel.ClickIncreaseMailsPerPageButtonCommand.Execute(null);
            WaitUntil.WaitUntilFuncIsTrue(
                () => (viewModel.OnPropertyCalledArgs.Count(p => p == "IsWindowEnabled") - isWindowsEnabledCount >= 4));
            Assert.AreEqual(1, viewModel.CurrentPageNumber);
            Assert.AreEqual(2, viewModel.Mails.Count);
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }
    }
}
