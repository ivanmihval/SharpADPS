using System.IO;

namespace WPFSharpADPS.Helpers.HashsumEngine
{
    public interface IHashsumEngine
    {
        HashsumEngineType GetEngineType();
        bool IsAvailable();
        string Calculate(Stream stream);
        string GetName();
    }
}
