using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CoreADPS;
using CoreADPS.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCoreADPS.TestHelpers
{
    [TestClass]
    public class HashsumCalculatorTests
    {
        public const string Content = "test12345";
        public const string ExpectedSha512HexDigest = "e6e8ba72bf2bab68c923599a08489c6f3a35018e870d490fa25b4c2a2d82361093b9a8f1d0cdd02e5a7dd5e714dc090263e2e90af3d3d861c056f28a4adb2d95";

        [TestMethod]
        public void TestSha512ChecksumBytes()
        {
            var hashsum = HashsumCalculator.Sha512Checksum(Unicode.Utf8WithouBom.GetBytes(Content));
            Assert.AreEqual(ExpectedSha512HexDigest, hashsum);
        }

        [TestMethod]
        public void TestSha512ChecksumPath()
        {
            var tmpFilePath = Path.GetTempFileName();
            File.WriteAllText(tmpFilePath, Content, Unicode.Utf8WithouBom);
            var hashsum = HashsumCalculator.Sha512Checksum(tmpFilePath);
            Assert.AreEqual(ExpectedSha512HexDigest, hashsum);

            File.Delete(tmpFilePath);
        }
    }
}
