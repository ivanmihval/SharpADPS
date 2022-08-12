using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFSharpADPS.Helpers.HashsumEngine
{
    public class HashsumEngineManager
    {
        public static IHashsumEngine GetHashsumEngine(HashsumEngineType hashsumEngineType)
        {
            switch (hashsumEngineType)
            {
                case HashsumEngineType.DotNet:
                    return new DotNetHashsumEngine();
                case HashsumEngineType.Openssl:
                    return new OpensslHashsumEngine();
                default:
                    throw new KeyNotFoundException();
            }
        }

        public static List<IHashsumEngine> GetHashsumEngines()
        {
            var result = new List<IHashsumEngine>();
            foreach (var hashsumEngineType in Enum.GetValues(typeof(HashsumEngineType)).Cast<HashsumEngineType>())
            {
                result.Add(GetHashsumEngine(hashsumEngineType));
            }
            return result;
        }

        public const HashsumEngineType DefaultHashsumEngineType = HashsumEngineType.Openssl;
    }
}
