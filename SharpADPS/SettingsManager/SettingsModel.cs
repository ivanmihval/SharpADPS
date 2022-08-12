using System;
using System.Collections.Generic;
using System.Linq;
using WPFSharpADPS.Helpers.HashsumEngine;

namespace WPFSharpADPS.SettingsManager
{
    public class SettingsModel
    {
        private const int LimitPreviousRepositories = 10;
        private const int LimitTranslations = 15;

        public string[] PreviousRepositoryPaths;
        public int MailsPerPage;

        public int OverviewViewWidth;
        public int OverviewViewHeight;

        public HashsumEngineType HashsumEngineType;

        public TranslationLoadingMode TranslationLoadingMode;
        public string TranslationKey;
        public string[] ExternalTranslationKeys;
        public string[] ExternalTranslationTitles;
        public string[] ExternalTranslationPaths;

        public static SettingsModel DefaultSettingsModel()
        {
            return new SettingsModel
                {
                    PreviousRepositoryPaths = new string[0],
                    OverviewViewHeight = 350,
                    OverviewViewWidth = 525,
                    MailsPerPage = 35,
                    HashsumEngineType = HashsumEngineType.Openssl,
                    TranslationLoadingMode = TranslationLoadingMode.Embedded,
                    TranslationKey = "en",
                    ExternalTranslationKeys = new string[0],
                    ExternalTranslationTitles = new string[0],
                    ExternalTranslationPaths = new string[0],
                };
        }

        public void CorrectTranslation(string path, string expectedKey, string expectedTitle)
        {
            var minLength = Math.Min(Math.Min(ExternalTranslationKeys.Length, ExternalTranslationTitles.Length),
                                     ExternalTranslationPaths.Length);

            for (var i = 0; i < Math.Min(minLength, LimitTranslations); i++)
            {
                if (ExternalTranslationPaths[i] == path 
                    && (ExternalTranslationKeys[i] != expectedKey 
                        || ExternalTranslationTitles[i] != expectedTitle))
                {
                    ExternalTranslationKeys[i] = expectedKey;
                    ExternalTranslationTitles[i] = expectedTitle;
                }
            }
        }

        public void RemoveExternalTranslation(string key)
        {
            try
            {
                var minLength = Math.Min(Math.Min(ExternalTranslationKeys.Length, ExternalTranslationTitles.Length),
                                         ExternalTranslationPaths.Length);

                var newKeys = new List<string>();
                var newTitles = new List<string>();
                var newPaths = new List<string>();

                for (var i = 0; i < Math.Min(minLength, LimitTranslations); i++)
                {
                    if (ExternalTranslationKeys[i] != key)
                    {
                        newKeys.Add(ExternalTranslationKeys[i]);
                        newTitles.Add(ExternalTranslationTitles[i]);
                        newPaths.Add(ExternalTranslationPaths[i]);
                    }
                }

                ExternalTranslationKeys = newKeys.ToArray();
                ExternalTranslationTitles = newTitles.ToArray();
                ExternalTranslationPaths = newPaths.ToArray();
            }
            catch (Exception)
            {
                ExternalTranslationKeys = new string[0];
                ExternalTranslationTitles = new string[0];
                ExternalTranslationPaths = new string[0];
            }
        }

        public void AddExternalTranslation(string key, string title, string path)
        {
            var minLength = Math.Min(Math.Min(ExternalTranslationKeys.Length, ExternalTranslationTitles.Length),
                                     ExternalTranslationPaths.Length);

            var newKeys = new List<string> {key};
            var newTitles = new List<string> {title};
            var newPaths = new List<string> {path};

            for (var i = 0; i < Math.Min(minLength, LimitTranslations - 1); i++)
            {
                if (ExternalTranslationKeys[i] != key && ExternalTranslationPaths[i] != path)
                {
                    newKeys.Add(ExternalTranslationKeys[i]);
                    newTitles.Add(ExternalTranslationTitles[i]);
                    newPaths.Add(ExternalTranslationPaths[i]);
                }
            }

            ExternalTranslationKeys = newKeys.ToArray();
            ExternalTranslationTitles = newTitles.ToArray();
            ExternalTranslationPaths = newPaths.ToArray();
        }

        public void AddRepository(string path)
        {
            var head = new [] {path};
            var tail = PreviousRepositoryPaths.Where(p => p != path).Take(LimitPreviousRepositories - 1).ToArray();
            PreviousRepositoryPaths = head.Concat(tail).ToArray();
        }

        public void RemoveRepository(string path)
        {
            PreviousRepositoryPaths = PreviousRepositoryPaths.Where(p => p != path).ToArray();
        }
    }
}
