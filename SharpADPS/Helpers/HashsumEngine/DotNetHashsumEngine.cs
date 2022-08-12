using System.IO;
using CoreADPS.Helpers;

namespace WPFSharpADPS.Helpers.HashsumEngine
{
    public class DotNetHashsumEngine: IHashsumEngine
    {
        public HashsumEngineType GetEngineType()
        {
            return HashsumEngineType.DotNet;
        }

        public bool IsAvailable()
        {
            return true;
        }

        public string Calculate(Stream stream)
        {
            return HashsumCalculator.Sha512Checksum(stream);
        }

        public string GetName()
        {
            return ".NET";
        }
    }
}
