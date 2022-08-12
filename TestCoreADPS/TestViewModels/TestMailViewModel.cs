using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CoreADPS;
using CoreADPS.Helpers;
using CoreADPS.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCoreADPS.TestHelpers;
using TestCoreADPS.TestViewModels.Providers;
using WPFSharpADPS.Helpers.HashsumEngine;
using WPFSharpADPS.SettingsManager;
using WPFSharpADPS.ViewModels;

namespace TestCoreADPS.TestViewModels
{
    public class MailViewTestModel: MailViewModel
    {
        public readonly List<string> OnPropertyCalledArgs = new List<string>();

        private void _onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyCalledArgs.Add(e.PropertyName);
        }

        public MailViewTestModel()
        {
            PropertyChanged += _onPropertyChanged;
            MessageBoxProvider = new MockedMessageBoxProvider();
            OpenFileDialogProvider = new MockedOpenFileDialogProvider();
            FolderBrowserDialogProvider = new MockedFolderBrowserDialogProvider();

            SettingsManager.Settings.HashsumEngineType = HashsumEngineType.DotNet;
        }

        public MailViewTestModel(string messagePath, string storagePath)
            : base(messagePath, storagePath)
        {
            PropertyChanged += _onPropertyChanged;
            MessageBoxProvider = new MockedMessageBoxProvider();
            OpenFileDialogProvider = new MockedOpenFileDialogProvider();
            FolderBrowserDialogProvider = new MockedFolderBrowserDialogProvider();

            SettingsManager.Settings.HashsumEngineType = HashsumEngineType.DotNet;
        }
    }

    [TestClass]
    public class TestMailViewModel
    {
        [TestMethod]
        public void TestDateCreatedNowCheckbox()
        {
            var viewModel = new MailViewTestModel();
            var initDateCreated = viewModel.Mail.DateCreated;

            Thread.Sleep(2000);
            viewModel.DatetimeCreatedIsNow = true;

            var newDateCreated = viewModel.Mail.DateCreated;
            Assert.IsTrue(newDateCreated > initDateCreated);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "DatetimeCreatedIsNow",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                    "DateCreatedDate",
                    "DateCreatedHour",
                    "DateCreatedMinute"
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}", 
                String.Join(", ", viewModel.OnPropertyCalledArgs));
        }

        [TestMethod]
        public void TestNameChanged()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel();
            // ReSharper restore UseObjectOrCollectionInitializer
            viewModel.MailName = "12345";
            var initMailSizeBytes = viewModel.JsonBytesLength;

            viewModel.MailName = "12345abcde";
            var newMailSizeBytes = viewModel.JsonBytesLength;

            Assert.AreEqual(viewModel.Mail.Name, viewModel.MailName);
            Assert.AreEqual(newMailSizeBytes - initMailSizeBytes, 5);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "MailName",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));
        }

        [TestMethod]
        public void TestAdditionalNotesChanged()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel();
            // ReSharper restore UseObjectOrCollectionInitializer
            viewModel.MailAdditionalNotes = "12345";
            var initMailSizeBytes = viewModel.JsonBytesLength;

            viewModel.MailAdditionalNotes = "12345abcde";
            var newMailSizeBytes = viewModel.JsonBytesLength;

            Assert.AreEqual(viewModel.Mail.AdditionalNotes, viewModel.MailAdditionalNotes);
            Assert.AreEqual(newMailSizeBytes - initMailSizeBytes, 5);

            viewModel.MailAdditionalNotesIsNull = true;
            Assert.IsTrue(viewModel.Mail.AdditionalNotes == null);
            Assert.AreEqual(viewModel.JsonBytesLength, initMailSizeBytes - 3);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "MailAdditionalNotes",
                    "MailAdditionalNotesIsNull",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));
        }

        [TestMethod]
        public void TestInlineMessageChanged()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel();
            // ReSharper restore UseObjectOrCollectionInitializer
            viewModel.MailInlineMessage = "12345";
            var initMailSizeBytes = viewModel.JsonBytesLength;

            viewModel.MailInlineMessage = "12345abcde";
            var newMailSizeBytes = viewModel.JsonBytesLength;

            Assert.AreEqual(viewModel.Mail.InlineMessage, viewModel.MailInlineMessage);
            Assert.AreEqual(newMailSizeBytes - initMailSizeBytes, 5);

            viewModel.MailInlineMessageIsNull = true;
            Assert.IsTrue(viewModel.Mail.InlineMessage == null);
            Assert.AreEqual(viewModel.JsonBytesLength, initMailSizeBytes - 3);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "MailInlineMessage",
                    "MailInlineMessageIsNull",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));
        }

        [TestMethod]
        public void TestAttachment()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestAttachment", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var mailFilePath = Directory.GetFiles(Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder))[0];

            var newFileContent = Unicode.Utf8WithouBom.GetBytes("555555555");
            const string newFileFilename = "file.txt";
            var newFilePath = Path.Combine(testFolderPath, newFileFilename);
            File.WriteAllBytes(newFilePath, newFileContent);

            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel(mailFilePath, sourceRepoFolderPath);
            // ReSharper restore UseObjectOrCollectionInitializer
            viewModel.OpenFileDialogProvider = new MockedOpenFileDialogProvider(newFilePath);

            Assert.AreEqual(viewModel.MailAttachments.Count, viewModel.Mail.Attachments.Count);
            var initAttachmentsCount = viewModel.Mail.Attachments.Count;
            var initSizeBytes = viewModel.JsonBytesLength;

            viewModel.ClickAddAttachmentButtonCommand.Execute(null);

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);  // Wait until the background worker starts
                if (!viewModel.IsProgressbarVisible)
                {
                    break;
                }
            }

            Assert.IsFalse(viewModel.IsProgressbarVisible);
            Assert.IsTrue(initSizeBytes < viewModel.JsonBytesLength);
            Assert.AreEqual(viewModel.MailAttachments.Count, viewModel.Mail.Attachments.Count);
            Assert.AreEqual(viewModel.MailAttachments.Count, initAttachmentsCount + 1);
            Assert.IsTrue(viewModel.Mail.Attachments.Count(
                a => a.HashsumHex == HashsumCalculator.Sha512Checksum(newFileContent)) == 1);
            Assert.AreEqual(((MockedOpenFileDialogProvider) viewModel.OpenFileDialogProvider).CalledTimes, 1);

            // Test delete of an attachment
            Assert.IsFalse(viewModel.ClickDeleteAttachmentButtonCommand.CanExecute(null));
            viewModel.MailAttachments.ElementAt(1).IsSelected = true;
            viewModel.IsAttachmentsListBoxSelected = true;
            Assert.IsTrue(viewModel.ClickDeleteAttachmentButtonCommand.CanExecute(null));
            viewModel.ClickDeleteAttachmentButtonCommand.Execute(null);
            Assert.AreEqual(viewModel.MailAttachments.Count, viewModel.Mail.Attachments.Count);
            Assert.AreEqual(viewModel.MailAttachments.Count, initAttachmentsCount);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "MailAttachments",
                    "IsProgressbarVisible",
                    "ProgressBarValue",
                    "ProgressBarText",
                    "MaxProgressBarValue",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }

        [TestMethod]
        public void TestCoordinates()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestCoordinates", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var mailFilePath = Directory.GetFiles(Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder))[0];

            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel(mailFilePath, sourceRepoFolderPath);
            // ReSharper restore UseObjectOrCollectionInitializer

            Assert.AreEqual(viewModel.LocationViewObjects.Count, viewModel.Mail.RecipientsCoordinates.Count);
            var initCoordinatesCount = viewModel.Mail.RecipientsCoordinates.Count;
            var initSizeBytes = viewModel.JsonBytesLength;

            Assert.IsFalse(viewModel.ClickAddCoordinatesButtonCommand.CanExecute(null));
            viewModel.CoordinatesTextBoxContent = "12.34, 56.789";
            Assert.IsTrue(viewModel.ClickAddCoordinatesButtonCommand.CanExecute(null));
            viewModel.ClickAddCoordinatesButtonCommand.Execute(null);

            Assert.IsTrue(viewModel.JsonBytesLength > initSizeBytes);
            Assert.AreEqual(viewModel.LocationViewObjects.Count, viewModel.Mail.RecipientsCoordinates.Count);
            Assert.AreEqual(viewModel.LocationViewObjects.Count, initCoordinatesCount + 1);
            Assert.IsTrue(viewModel.Mail.RecipientsCoordinates
                .Where(c => Math.Abs(c.Latitude - 12.34) < 0.0001)
                .Any(c => Math.Abs(c.Longitude - 56.789) < 0.0001));

            // Test delete of a location
            Assert.IsFalse(viewModel.ClickDeleteCoordinatesButtonCommand.CanExecute(null));
            viewModel.LocationViewObjects.ElementAt(1).IsSelected = true;
            viewModel.IsCoordinatesListBoxSelected = true;
            Assert.IsTrue(viewModel.ClickDeleteCoordinatesButtonCommand.CanExecute(null));
            viewModel.ClickDeleteCoordinatesButtonCommand.Execute(null);
            Assert.AreEqual(viewModel.LocationViewObjects.Count, viewModel.Mail.RecipientsCoordinates.Count);
            Assert.AreEqual(viewModel.LocationViewObjects.Count, initCoordinatesCount);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "LocationViewObjects",
                    "Mail",
                    "JsonBytesLength",
                    "MessageFileIsTooBig",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }

        [TestMethod]
        public void TestExportToFolder()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestExportToFolder", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";
            const string exportFolderName = "exported";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var mailFilePath = Directory.GetFiles(Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder))[0];

            var exportFolderPath = Path.Combine(testFolderPath, exportFolderName);
            Directory.CreateDirectory(exportFolderPath);

            // Check messagebox on non-empty folder
            var newFileContent = Unicode.Utf8WithouBom.GetBytes("555555555");
            const string newFileFilename = "file.txt";
            var newFilePath = Path.Combine(exportFolderPath, newFileFilename);
            File.WriteAllBytes(newFilePath, newFileContent);

            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel(mailFilePath, sourceRepoFolderPath);
            // ReSharper restore UseObjectOrCollectionInitializer
            viewModel.FolderBrowserDialogProvider = new MockedFolderBrowserDialogProvider(exportFolderPath);

            viewModel.MailViewModelMode = MailViewModelMode.EditExisting;
            Assert.IsFalse(viewModel.ClickExportToFolderButtonCommand.CanExecute(null));
            viewModel.MailViewModelMode = MailViewModelMode.ViewExisting;
            Assert.IsTrue(viewModel.ClickExportToFolderButtonCommand.CanExecute(null));

            viewModel.ClickExportToFolderButtonCommand.Execute(null);
            Assert.AreEqual(((MockedMessageBoxProvider) viewModel.MessageBoxProvider).Calls.Count, 1);
            Assert.AreEqual(((MockedMessageBoxProvider) viewModel.MessageBoxProvider).Calls.ElementAt(0).Item1,
                            "The folder should be empty");

            File.Delete(newFilePath);
            viewModel.ClickExportToFolderButtonCommand.Execute(null);

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);  // Wait until the background worker starts
                if (!viewModel.IsProgressbarVisible)
                {
                    break;
                }
            }

            Assert.IsFalse(viewModel.IsProgressbarVisible);
            Assert.AreEqual(Directory.GetFiles(exportFolderPath).Length, viewModel.Mail.Attachments.Count + 1);
            Assert.AreEqual(((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls.Count, 2);
            Assert.AreEqual(((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls.ElementAt(1).Item1,
                            "The operation is finished successfully!");

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "IsProgressbarIndeterminate",
                    "IsProgressbarVisible",
                    "ProgressBarValue",
                    "ProgressBarText",
                    "MaxProgressBarValue",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }

        [TestMethod]
        public void TestSaveMail()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestSaveMail", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var messageFolderPath = Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder);
            var fileList = Directory.GetFiles(messageFolderPath);
            var initFilesInFolderCount = fileList.Length;
            var mailFilePath = fileList[0];

            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel(mailFilePath, sourceRepoFolderPath);
            // ReSharper restore UseObjectOrCollectionInitializer

            viewModel.MailViewModelMode = MailViewModelMode.ViewExisting;
            Assert.IsFalse(viewModel.ClickSaveMessageButtonCommand.CanExecute(null));
            viewModel.MailViewModelMode = MailViewModelMode.EditExisting;
            Assert.IsFalse(viewModel.ClickSaveMessageButtonCommand.CanExecute(null));
            viewModel.MailName += "123";
            Assert.IsTrue(viewModel.ClickSaveMessageButtonCommand.CanExecute(null));
            viewModel.Mail.InlineMessage = new string('*', 4096);
            Assert.IsFalse(viewModel.ClickSaveMessageButtonCommand.CanExecute(null));
            viewModel.Mail.InlineMessage = "Inline Msg12345";

            viewModel.ClickSaveMessageButtonCommand.Execute(null);

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);  // Wait until the background worker starts
                if (!viewModel.IsProgressbarVisible)
                {
                    break;
                }
            }

            Assert.AreEqual(viewModel.MailViewModelMode, MailViewModelMode.ViewExisting);
            Assert.IsFalse(viewModel.IsProgressbarVisible);
            Assert.AreEqual(Directory.GetFiles(messageFolderPath).Length, initFilesInFolderCount + 1);
            Assert.AreEqual(((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls.Count, 1);
            Assert.AreEqual(((MockedMessageBoxProvider)viewModel.MessageBoxProvider).Calls.ElementAt(0).Item1,
                            "The operation is finished successfully!");

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "IsProgressbarIndeterminate",
                    "IsProgressbarVisible",
                    "ProgressBarText",
                    "MailViewModelMode",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }

        [TestMethod]
        public void TestChangeLockStatus()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestChangeLockStatus", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var mailFilePath = Directory.GetFiles(Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder))[0];

            // ReSharper disable UseObjectOrCollectionInitializer
            var viewModel = new MailViewTestModel(mailFilePath, sourceRepoFolderPath);
            // ReSharper restore UseObjectOrCollectionInitializer

            viewModel.MailViewModelMode = MailViewModelMode.New;
            Assert.IsFalse(viewModel.ClickChangeLockStatusButtonCommand.CanExecute(null));

            viewModel.MailViewModelMode = MailViewModelMode.ViewExisting;
            Assert.IsTrue(viewModel.ClickChangeLockStatusButtonCommand.CanExecute(null));
            viewModel.ClickChangeLockStatusButtonCommand.Execute(null);
            Assert.AreEqual(viewModel.MailViewModelMode, MailViewModelMode.EditExisting);

            Assert.IsTrue(viewModel.ClickChangeLockStatusButtonCommand.CanExecute(null));
            viewModel.ClickChangeLockStatusButtonCommand.Execute(null);
            Assert.AreEqual(viewModel.MailViewModelMode, MailViewModelMode.ViewExisting);

            var actualCalledArgs = new HashSet<string>(viewModel.OnPropertyCalledArgs);
            var expectedCalledArgs = new HashSet<string>
                {
                    "MailViewModelMode",
                };
            var isSubset = !expectedCalledArgs.Except(actualCalledArgs).Any();
            Assert.IsTrue(isSubset, "Not enough OnPropertyChange invokations: {0}",
                String.Join(", ", viewModel.OnPropertyCalledArgs));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }
    }
}
