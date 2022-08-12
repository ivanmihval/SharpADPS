using System.Text;

namespace CoreADPS
{
    public static class Unicode
    {
        public static Encoding Utf8WithouBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
