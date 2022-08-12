using System;
using System.IO;
using System.Xml.Serialization;
using WPFSharpADPS.Helpers.HashsumEngine;

namespace WPFSharpADPS.SettingsManager
{
    public static class SettingsManager
    {
        public static string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WpfAdpsConfig.xml");

        private static SettingsModel _loadSettings()
        {
            var serializer = new XmlSerializer(typeof(SettingsModel));
            try
            {
                using (var fs = new FileStream(SettingsPath, FileMode.OpenOrCreate))
                {
                    var result = serializer.Deserialize(fs);
                    return (result as SettingsModel) ?? SettingsModel.DefaultSettingsModel();
                }
            }
            catch (Exception)
            {
                return SettingsModel.DefaultSettingsModel();
            }
        }

        public static void SaveSettings()
        {
            var serializer = new XmlSerializer(typeof(SettingsModel));
            using (var fs = new FileStream(SettingsPath, FileMode.Create))
            {
                serializer.Serialize(fs, Settings);
            }
        }

        public static SettingsModel Settings = _loadSettings();

        public static IHashsumEngine GetHashsumEngine()
        {
            return HashsumEngineManager.GetHashsumEngine(Settings.HashsumEngineType);
        }
    }
}
