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
    public class TestCopyMails
    {
        public const string PartialCollisionFile1 = @"
            ewogICAgImFkZGl0aW9uYWxfbm90ZXMiOiBudWxsLAogICAgImF0dGFjaG1lbnRzIjogWwogICAgICAgIHsKICAgIC
            AgICAgICAgImZpbGVuYW1lIjogIjMzMy50eHQiLAogICAgICAgICAgICAiaGFzaHN1bV9hbGciOiAic2hhNTEyIiwK
            ICAgICAgICAgICAgImhhc2hzdW1faGV4IjogImE4OTc5NGFlYmE3NWVjM2I3MTE3MzVmMTc1Nzc3NGY1ZWExN2E0Yj
            RiOTdjNjljYmIwMGRlMGUzMmZhOTU2ZWZmZjI1YzkxYmQ3ZjRmMWE5NTU3YTE3ZTdlZTJjM2FiMmYxNDFhNDhiODk5
            YjI0MjA1NDlmYzUxZjhiMjcwZDkxIiwKICAgICAgICAgICAgInNpemVfYnl0ZXMiOiA0NgogICAgICAgIH0KICAgIF
            0sCiAgICAiZGF0ZV9jcmVhdGVkIjogIjIwMjItMDItMDJUMDA6MDA6MDAiLAogICAgImlubGluZV9tZXNzYWdlIjog
            IkZyYW5rIGlzIG9uZSBvZiB0aGUgdG9wIHN0dWRlbnRzIHRvcGljSWQxMTM1NyAiLAogICAgIm1pbl92ZXJzaW9uIj
            ogIjEuMCIsCiAgICAibmFtZSI6ICJKb2hubnkiLAogICAgInJlY2lwaWVudF9jb29yZHMiOiBbCiAgICAgICAgewog
            ICAgICAgICAgICAibGF0IjogMS4wLAogICAgICAgICAgICAibG9uIjogMi4wCiAgICAgICAgfQogICAgXSwKICAgIC
            J2ZXJzaW9uIjogIjEuMCIKfQ==";

        public const string PartialCollisionFile2 = @"
            ewogICAgImFkZGl0aW9uYWxfbm90ZXMiOiBudWxsLAogICAgImF0dGFjaG1lbnRzIjogWwogICAgICAgIHsKICAgIC
            AgICAgICAgImZpbGVuYW1lIjogIjEyMy50eHQiLAogICAgICAgICAgICAiaGFzaHN1bV9hbGciOiAic2hhNTEyIiwK
            ICAgICAgICAgICAgImhhc2hzdW1faGV4IjogImE4OTc5NGFlYmEwYmI3ZGY2ZTg3M2ZlNWFjOTFjZTY1MmJiMGY2Nm
            E4YTBmYmQ3OWUzNjJmN2I4YzhlOGM3OGFjOWZjYjg2NjYwZjc4ODIwYjUzN2ZkMjlhY2UwZDczYzA3NzM0OTllOTM2
            M2Y1YzUyNDgzYmFkYzkwZDgyNGJhIiwKICAgICAgICAgICAgInNpemVfYnl0ZXMiOiA0OAogICAgICAgIH0KICAgIF
            0sCiAgICAiZGF0ZV9jcmVhdGVkIjogIjIwMjItMDItMDJUMDA6MDA6MDAiLAogICAgImlubGluZV9tZXNzYWdlIjog
            IkZyYW5rIGlzIG9uZSBvZiB0aGUgYmVzdCBzdHVkZW50cyB0b3BpY0lkMjUyNTA1MCAiLAogICAgIm1pbl92ZXJzaW
            9uIjogIjEuMCIsCiAgICAibmFtZSI6ICJKb2hubnkiLAogICAgInJlY2lwaWVudF9jb29yZHMiOiBbCiAgICAgICAg
            ewogICAgICAgICAgICAibGF0IjogMS4wLAogICAgICAgICAgICAibG9uIjogMi4wCiAgICAgICAgfQogICAgXSwKIC
            AgICJ2ZXJzaW9uIjogIjEuMCIKfQ==
            ";

        public const string PartialCollisionFile3 = "RnJhbmsgaXMgb25lIG9mIHRoZSB0b3Agc3R1ZGVudHMgdG9waWNJZDI4NDA1IA==";
        public const string PartialCollisionFile4 = "RnJhbmsgaXMgb25lIG9mIHRoZSBiZXN0IHN0dWRlbnRzIHRvcGljSWQ3MjA5Mjgg";


        [TestMethod]
        public void TestOkPartialCollisions()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestOkPartialCollisions", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";
            const string targetFolderName = "target";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            var sourceMessagesFolderPath = Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder);
            var sourceAttachmentsFolderPath = Path.Combine(sourceRepoFolderPath, Storage.AttachmentsFolder);

            var targetRepoFolderPath = Path.Combine(testFolderPath, targetFolderName);
            var targetMessagesFolderPath = Path.Combine(targetRepoFolderPath, Storage.MessagesFolder);
            var targetAttachmentsFolderPath = Path.Combine(targetRepoFolderPath, Storage.AttachmentsFolder);

            Directory.CreateDirectory(targetMessagesFolderPath);
            Directory.CreateDirectory(targetAttachmentsFolderPath);

            GenerateTestRepo.Generate(sourceRepoFolderPath);
            File.WriteAllBytes(Path.Combine(sourceMessagesFolderPath, "c850eddde6_0000.json"), Convert.FromBase64String(PartialCollisionFile1));
            File.WriteAllBytes(Path.Combine(sourceMessagesFolderPath, "c850eddde6.json"), Convert.FromBase64String(PartialCollisionFile2));
            File.WriteAllBytes(Path.Combine(sourceAttachmentsFolderPath, "a89794aeba_0000.bin"), Convert.FromBase64String(PartialCollisionFile3));
            File.WriteAllBytes(Path.Combine(sourceAttachmentsFolderPath, "a89794aeba.bin"), Convert.FromBase64String(PartialCollisionFile4));

            var storage = new Storage(sourceRepoFolderPath);

            var sourceMessagesFileList = Directory.GetFiles(sourceMessagesFolderPath).Select(Path.GetFileName).OrderBy(v => v).ToArray();
            Assert.IsTrue(sourceMessagesFileList.SequenceEqual(new[] { "0d948fdc77.json", "bee12b5bd6.json", "c850eddde6.json", "c850eddde6_0000.json" }));

            var sourceAttachmentsFileList = Directory.GetFiles(sourceAttachmentsFolderPath).Select(Path.GetFileName).OrderBy(v => v).ToArray();
            Assert.AreEqual(5, sourceAttachmentsFileList.Length);
            Assert.IsTrue(sourceAttachmentsFileList.SequenceEqual(new[] { "158911a346.bin", "3627909a29.bin", "a89794aeba.bin", "a89794aeba_0000.bin", "e711a66e46.bin" }));

            storage.CopyMails(
                sourceMessagesFileList,
                targetRepoFolderPath, 
                new DotNetHashsumEngine().Calculate
                );

            var targetMessagesFileList = Directory.GetFiles(targetMessagesFolderPath).Select(Path.GetFileName).OrderBy(v => v).ToArray();
            Assert.IsTrue(new HashSet<string>(sourceMessagesFileList).SetEquals(targetMessagesFileList));
           
            var targetAttachmentsFileList = Directory.GetFiles(targetAttachmentsFolderPath).Select(Path.GetFileName).OrderBy(v => v).ToArray();
            Assert.IsTrue(new HashSet<string>(sourceAttachmentsFileList).SetEquals(targetAttachmentsFileList));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }

        [TestMethod]
        public void TestOk()
        {
            var temporaryFolder = Path.GetTempPath();
            var testFolderName = String.Format("{0}_adps_repo__TestCopyMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var testFolderPath = Path.Combine(temporaryFolder, testFolderName);

            const string sourceFolderName = "source";
            const string targetFolderName = "target";

            var sourceRepoFolderPath = Path.Combine(testFolderPath, sourceFolderName);
            var sourceMessagesFolderPath = Path.Combine(sourceRepoFolderPath, Storage.MessagesFolder);
            var sourceAttachmentsFolderPath = Path.Combine(sourceRepoFolderPath, Storage.AttachmentsFolder);

            var targetRepoFolderPath = Path.Combine(testFolderPath, targetFolderName);
            var targetMessagesFolderPath = Path.Combine(targetRepoFolderPath, Storage.MessagesFolder);
            var targetAttachmentsFolderPath = Path.Combine(targetRepoFolderPath, Storage.AttachmentsFolder);

            Directory.CreateDirectory(targetMessagesFolderPath);
            Directory.CreateDirectory(targetAttachmentsFolderPath);

            GenerateTestRepo.Generate(sourceRepoFolderPath);
            var storage = new Storage(sourceRepoFolderPath);

            var sourceMessagesFileList = Directory.GetFiles(sourceMessagesFolderPath).Select(Path.GetFileName).OrderBy(v => v);
            Assert.IsTrue(sourceMessagesFileList.SequenceEqual(new[] { "0d948fdc77.json", "bee12b5bd6.json" }));

            File.Move(
                Path.Combine(sourceAttachmentsFolderPath, "158911a346.bin"),
                Path.Combine(sourceAttachmentsFolderPath, "158911a346_attachment.bin")
                );

            storage.CopyMails(
                new[] { "bee12b5bd6.json" },
                targetRepoFolderPath, new DotNetHashsumEngine().Calculate);

            var targetMessagesFileList = Directory.GetFiles(targetMessagesFolderPath).Select(Path.GetFileName);
            Assert.IsTrue(targetMessagesFileList.SequenceEqual(new[] { "bee12b5bd6.json" }));

            var targetAttachmentsFileList = Directory.GetFiles(targetAttachmentsFolderPath).Select(Path.GetFileName).OrderBy(v => v);
            Assert.IsTrue(targetAttachmentsFileList.SequenceEqual(new[] { "158911a346.bin", "3627909a29.bin" }));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(testFolderPath, true);
        }
    }
}
