using System.IO;
using CoreADPS.Helpers;

namespace WPFSharpADPS.Helpers.HashsumEngine
{
    public class OpensslHashsumEngine: IHashsumEngine
    {
        public HashsumEngineType GetEngineType()
        {
            return HashsumEngineType.Openssl;
        }

        public bool IsAvailable()
        {
            return true;
        }

        public string Calculate(Stream stream)
        {
            return HashsumCalculator.Sha512ChecksumOpenssl(stream);
        }

        public string GetName()
        {
            return "OpenSSL";
        }
    }
}
