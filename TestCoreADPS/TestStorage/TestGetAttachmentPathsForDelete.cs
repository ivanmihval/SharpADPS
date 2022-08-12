using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreADPS.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCoreADPS.TestHelpers;
using WPFSharpADPS.Helpers.HashsumEngine;

namespace TestCoreADPS.TestStorage
{
    [TestClass]
    public class TestGetAttachmentPathsForDelete
    {
        [TestMethod]
        public void TestOk()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestGetAttachmentPathsForDelete", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);
            var messagesPath = Path.Combine(repoFolderPath, Storage.MessagesFolder);
            var attachmentsPath = Path.Combine(repoFolderPath, Storage.AttachmentsFolder);

            var mails = GenerateTestRepo.Generate(repoFolderPath);
            Assert.AreEqual(mails.Count, 2);

            var sourceMessagesFileList = Directory.GetFiles(messagesPath).Select(Path.GetFileName).OrderBy(v => v);
            Assert.IsTrue(sourceMessagesFileList.SequenceEqual(new[] {"0d948fdc77.json", "bee12b5bd6.json"}));

            var sourceAttachmentFileList = Directory.GetFiles(attachmentsPath).Select(Path.GetFileName).OrderBy(v => v);
            Assert.IsTrue(sourceAttachmentFileList.SequenceEqual(new[] {"158911a346.bin", "3627909a29.bin", "e711a66e46.bin"}));

            var storage = new Storage(repoFolderPath);
            var attachmentPaths = storage.GetAttachmentPathsForDelete(new[] { "0d948fdc77.json" }, new DotNetHashsumEngine().Calculate);
            var attachmentNames = attachmentPaths.Select(Path.GetFileName).OrderBy(v => v);
            Assert.IsTrue(attachmentNames.SequenceEqual(new[] {"e711a66e46.bin"}));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }

        [TestMethod]
        public void TestRenameEmpty()
        {
            var pathsToDelete = new List<string>
                {
                    "C:/adps_repo/adps_attachments/1234567890.bin",
                    "C:/adps_repo/adps_attachments/12345678a0.bin",
                    "C:/adps_repo/adps_attachments/ab345678a0.bin",
                    "C:/adps_repo/adps_attachments/abc45678a0.bin",
                };

            var cachedAttachmentsFilenames = new List<string>
                {
                    "1234567890.bin",
                    "12345678a0.bin",
                    "ab345678a0.bin",
                    "abc45678a0.bin",
                    "12345b7890.bin",
                    "12345b78a0.bin",
                    "ab345b78a0.bin",
                    "abc45b78a0.bin",
                };

            var filenamesAfterDelete =
                cachedAttachmentsFilenames.Where(fn => !pathsToDelete.Select(Path.GetFileName).Contains(fn)).ToList();

            var storage = new Storage("C:/adps_repo");
            var result = storage.GetAttachmentRenameMapping(filenamesAfterDelete);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void TestGetAttachmentRenameMappingOk()
        {
            var pathsToDelete = new List<string>
                {
                    "C:/adps_repo/adps_attachments/1234567890_0000.bin",
                    // The next 2 filenames is a "tail" so there is no need to rename files with their partial hashsum
                    "C:/adps_repo/adps_attachments/12345678a0_0001.bin",
                    "C:/adps_repo/adps_attachments/12345678a0_0000.bin",
                    "C:/adps_repo/adps_attachments/abc45678a0.bin",
                };

            var cachedAttachmentsFilenames = new List<string>
                {
                    "1234567890.bin",
                    "12345678a0.bin",
                    "ab345678a0.bin",
                    "abc45678a0.bin",
                    "abc45678a0_0000.bin",
                    "1234567890_0000.bin",
                    "1234567890_0001.bin",
                    "1234567890_0002.bin",
                    "12345678a0_0000.bin",
                    "12345678a0_0001.bin",
                };

            var filenamesAfterDelete =
                cachedAttachmentsFilenames.Where(fn => !pathsToDelete.Select(Path.GetFileName).Contains(fn)).ToList();

            var storage = new Storage("C:/adps_repo");
            var result = storage.GetAttachmentRenameMapping(filenamesAfterDelete);
            Assert.AreEqual(result.Count, 3);

            Assert.AreEqual(Path.GetFileName(result.ElementAt(0).Item1), "1234567890_0001.bin");
            Assert.AreEqual(Path.GetFileName(result.ElementAt(0).Item2), "1234567890_0000.bin");

            Assert.AreEqual(Path.GetFileName(result.ElementAt(1).Item1), "1234567890_0002.bin");
            Assert.AreEqual(Path.GetFileName(result.ElementAt(1).Item2), "1234567890_0001.bin");

            Assert.AreEqual(Path.GetFileName(result.ElementAt(2).Item1), "abc45678a0_0000.bin");
            Assert.AreEqual(Path.GetFileName(result.ElementAt(2).Item2), "abc45678a0.bin");
        }
    }
}
