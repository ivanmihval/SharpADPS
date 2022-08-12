using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using OpenSSL.Core;
using OpenSSL.Crypto;

namespace CoreADPS.Helpers
{
    public class HashsumCalculator
    {

        private static string _hashsumBytesToString(IEnumerable<byte> hashedInputBytes)
        {
            // Convert to text
            // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte
            var hashedInputStringBuilder = new System.Text.StringBuilder(128);
            foreach (var b in hashedInputBytes)
            {
                hashedInputStringBuilder.Append(b.ToString("x2"));
            }
            return hashedInputStringBuilder.ToString();
        }

        public static string Sha512Checksum(Stream stream)
        {
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(stream);
                return _hashsumBytesToString(hashedInputBytes);
            }
        }

        public static string Sha512Checksum(string filePath, int bufferSize = 1024*1024)
        {
            using (var fileStream = new BufferedStream(File.OpenRead(filePath), bufferSize))
            {
                return Sha512Checksum(fileStream);
            }
        }

        public static string Sha512ChecksumOpenssl(Stream fileStream, int bufferSize = 1024*1024)
        {
            var messageDigestContext = new MessageDigestContext(MessageDigest.SHA512);
            messageDigestContext.Init();

            var bytesArray = new byte[bufferSize];
            int readBytes;

            do
            {
                readBytes = fileStream.Read(bytesArray, 0, bufferSize);
                messageDigestContext.Update(readBytes == bufferSize ? bytesArray : bytesArray.Take(readBytes).ToArray());
            } while (readBytes == bufferSize);

            var hashsumBytes = messageDigestContext.DigestFinal();
            return _hashsumBytesToString(hashsumBytes);
        }

        // https://stackoverflow.com/a/39131803
        public static string Sha512Checksum(byte[] bytes)
        {
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                return _hashsumBytesToString(hashedInputBytes);
            }
        }
    }
}
