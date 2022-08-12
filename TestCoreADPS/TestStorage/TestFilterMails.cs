using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CoreADPS.Filters;
using CoreADPS.MailModels;
using CoreADPS.Storage;
using CoreADPS.Storage.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCoreADPS.TestHelpers;

namespace TestCoreADPS.TestStorage
{
    [TestClass]
    public class TestFilterMails
    {
        [TestMethod]
        public void TestOk()
        {
            var temporaryFolder = Path.GetTempPath();
            var repoFolderName = String.Format("{0}_adps_repo__TestFilterMails", DateTime.Now.ToString("yyyyMMdd'T'HHmmss"));
            var repoFolderPath = Path.Combine(temporaryFolder, repoFolderName);

            var mails = GenerateTestRepo.Generate(repoFolderPath);
            Assert.AreEqual(mails.Count, 2);
            var mail1 = mails.ElementAt(0);

            var storage = new Storage(repoFolderPath);

            var filteredMailJsons =
                storage.FilteredMailResults(new List<IMailParamFilter>
                    {
                        new DateTimeCreatedRangeFilter(dateTimeFrom: new DateTime(2019, 12, 1))
                    })
                    .Select(res => res.Mail.ToJson())
                    .ToArray();

            Assert.IsTrue(filteredMailJsons.SequenceEqual(new[] { mail1.ToJson() }));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(repoFolderPath, true);
        }
    }
}
