using System;
using System.Collections.Generic;
using System.Linq;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.SettingsManager;

namespace WPFSharpADPS.Models.Translation
{
    public class TranslationMenuItem : PropertyChangedModel
    {
        public string Key { get; private set; }
        public string Title { get; private set; }
        public TranslationLoadingMode Mode { get; private set; }

        public string Caption { get { return String.Format("{0} ({1})", Title, Mode); } }

        public bool IsSelected
        {
            get
            {
                return Mode == SettingsManager.SettingsManager.Settings.TranslationLoadingMode &&
                       Key == SettingsManager.SettingsManager.Settings.TranslationKey;
            }
        }

        public TranslationMenuItem(string key, string title, TranslationLoadingMode mode)
        {
            Key = key;
            Title = title;
            Mode = mode;
        }

        public static List<TranslationMenuItem> GetAllTranslationMenuItems()
        {
            var result = new List<TranslationMenuItem>();
            result.AddRange(
                TranslationModel.GetDefaultTranslations()
                                .Select(tm => new TranslationMenuItem(tm.Key, tm.Title, TranslationLoadingMode.Embedded)));


            var minLength =
                Math.Min(
                    Math.Min(SettingsManager.SettingsManager.Settings.ExternalTranslationKeys.Length,
                             SettingsManager.SettingsManager.Settings.ExternalTranslationTitles.Length),
                    SettingsManager.SettingsManager.Settings.ExternalTranslationPaths.Length);

            for (var i = 0; i < minLength; i++)
            {
                var key = SettingsManager.SettingsManager.Settings.ExternalTranslationKeys[i];
                var title = SettingsManager.SettingsManager.Settings.ExternalTranslationTitles[i];
                result.Add(new TranslationMenuItem(key, title, TranslationLoadingMode.ExternalFile));
            }

            return result;
        }
    }
}
