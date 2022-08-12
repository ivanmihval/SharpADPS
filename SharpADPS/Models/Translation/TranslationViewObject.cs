using System;
using System.IO;
using System.Xml.Serialization;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.SettingsManager;

namespace WPFSharpADPS.Models.Translation
{
    public class TranslationViewObject : PropertyChangedModel
    {
        public readonly TranslationModel TranslationModel;

        private string _getValueFromTranslationModelOrDefault(string propertyName)
        {
            if (TranslationModel != null)
            {
                //var t = _translationModel.GetType();
                //var prop = t.GetProperty("OverviewFilesMenuTitle");
                //var props = t.GetFields();

                var translationValue = TranslationModel
                                           .GetType()
                                           .GetField(propertyName)
                                           .GetValue(TranslationModel) as string;

                if (!string.IsNullOrWhiteSpace(translationValue))
                {
                    return translationValue;
                }
            }

            var defaultTranslationModel = TranslationModel.GetDefaultEnglish();
            var defaultValue = defaultTranslationModel
                                   .GetType()
                                   .GetField(propertyName)
                                   .GetValue(defaultTranslationModel) as string;

            return defaultValue ?? "Undefined";
        }

        // notepad++
        // search pattern: "public string (\w+);" (from TranslationModel.cs)
        // replace pattern: "public string \1 { get { return _getValueFromTranslationModelOrDefault\("\1"\); } }"

        //public string OverviewFilesMenuTitle
        //{
        //    get { return _getValueFromTranslationModelOrDefault("OverviewFilesMenuTitle"); }
        //}

        public string Key { get { return _getValueFromTranslationModelOrDefault("Key"); } }
        public string Title { get { return _getValueFromTranslationModelOrDefault("Title"); } }

        public string OverviewFilesMenuTitle { get { return _getValueFromTranslationModelOrDefault("OverviewFilesMenuTitle"); } }
        public string OverviewNewRepositoryTitle { get { return _getValueFromTranslationModelOrDefault("OverviewNewRepositoryTitle"); } }
        public string OverviewLoadRepositoryTitle { get { return _getValueFromTranslationModelOrDefault("OverviewLoadRepositoryTitle"); } }
        public string OverviewNewMailTitle { get { return _getValueFromTranslationModelOrDefault("OverviewNewMailTitle"); } }
        public string OverviewCopyToRepositoryTitle { get { return _getValueFromTranslationModelOrDefault("OverviewCopyToRepositoryTitle"); } }
        public string OverviewDeleteFromRepositoryTitle { get { return _getValueFromTranslationModelOrDefault("OverviewDeleteFromRepositoryTitle"); } }
        public string OverviewRecentRepositoriesTitle { get { return _getValueFromTranslationModelOrDefault("OverviewRecentRepositoriesTitle"); } }
        public string OverviewExitTitle { get { return _getValueFromTranslationModelOrDefault("OverviewExitTitle"); } }

        public string OverviewSelectionMenuTitle { get { return _getValueFromTranslationModelOrDefault("OverviewSelectionMenuTitle"); } }
        public string OverviewRemoveSelectedItemsTitle { get { return _getValueFromTranslationModelOrDefault("OverviewRemoveSelectedItemsTitle"); } }
        public string OverviewLeaveSelectedItemsTitle { get { return _getValueFromTranslationModelOrDefault("OverviewLeaveSelectedItemsTitle"); } }

        public string OverviewFiltersMenuTitle { get { return _getValueFromTranslationModelOrDefault("OverviewFiltersMenuTitle"); } }
        public string OverviewResetFiltersTitle { get { return _getValueFromTranslationModelOrDefault("OverviewResetFiltersTitle"); } }
        public string OverviewApplyFiltersTitle { get { return _getValueFromTranslationModelOrDefault("OverviewApplyFiltersTitle"); } }

        public string OverviewSettingsMenuTitle { get { return _getValueFromTranslationModelOrDefault("OverviewSettingsMenuTitle"); } }

        public string OverviewHelpMenuTitle { get { return _getValueFromTranslationModelOrDefault("OverviewHelpMenuTitle"); } }
        public string OverviewDocumentationTitle { get { return _getValueFromTranslationModelOrDefault("OverviewDocumentationTitle"); } }
        public string OverviewAboutTitle { get { return _getValueFromTranslationModelOrDefault("OverviewAboutTitle"); } }

        public string OverviewDataGridDateHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridDateHeader"); } }
        public string OverviewDataGridCoordinatesHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridCoordinatesHeader"); } }
        public string OverviewDataGridNameHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridNameHeader"); } }
        public string OverviewDataGridAttachmentsHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridAttachmentsHeader"); } }
        public string OverviewDataGridAdditionalNotesHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridAdditionalNotesHeader"); } }
        public string OverviewDataGridInlineMessageHeader { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridInlineMessageHeader"); } }

        public string OverviewDataGridContextMenuOpen { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridContextMenuOpen"); } }

        public string OverviewCloseRepoButtonText { get { return _getValueFromTranslationModelOrDefault("OverviewCloseRepoButtonText"); } }
        public string OverviewShowAllMailsButtonText { get { return _getValueFromTranslationModelOrDefault("OverviewShowAllMailsButtonText"); } }
        public string OverviewIncreaseRowsPerPageTooltip { get { return _getValueFromTranslationModelOrDefault("OverviewIncreaseRowsPerPageTooltip"); } }
        public string OverviewDecreaseRowsPerPageTooltip { get { return _getValueFromTranslationModelOrDefault("OverviewDecreaseRowsPerPageTooltip"); } }
        public string OverviewChangeRowsPerPageTooltipCurrentValue { get { return _getValueFromTranslationModelOrDefault("OverviewChangeRowsPerPageTooltipCurrentValue"); } }
        public string OverviewTotalMessagesTextBlock { get { return _getValueFromTranslationModelOrDefault("OverviewTotalMessagesTextBlock"); } }
        public string OverviewNextPageTooltip { get { return _getValueFromTranslationModelOrDefault("OverviewNextPageTooltip"); } }
        public string OverviewPreviousPageTooltip { get { return _getValueFromTranslationModelOrDefault("OverviewPreviousPageTooltip"); } }

        public string OverviewNoRepoLoadedTitle { get { return _getValueFromTranslationModelOrDefault("OverviewNoRepoLoadedTitle"); } }

        public string OverviewDataGridDaysAgoTemplate { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridDaysAgoTemplate"); } }
        public string OverviewDataGridNoCoordinates { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridNoCoordinates"); } }
        public string OverviewDataGridManyCoordinatesTemplate { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridManyCoordinatesTemplate"); } }
        public string OverviewDataGridNoFiles { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridNoFiles"); } }
        public string OverviewDataGridManyFilesTemplate { get { return _getValueFromTranslationModelOrDefault("OverviewDataGridManyFilesTemplate"); } }

        public string OverviewProgressBarFilteringMessages { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarFilteringMessages"); } }
        public string OverviewProgressBarCopyEstimation { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarCopyEstimation"); } }
        public string OverviewProgressBarCopying { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarCopying"); } }
        public string OverviewProgressBarScanningTargetFiles { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarScanningTargetFiles"); } }
        public string OverviewProgressBarScanningAllFiles { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarScanningAllFiles"); } }
        public string OverviewProgressBarDeleteFiles { get { return _getValueFromTranslationModelOrDefault("OverviewProgressBarDeleteFiles"); } }

        public string OverviewMessageBoxConfirmationCaption { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxConfirmationCaption"); } }
        public string OverviewMessageBoxNotificationCaption { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxNotificationCaption"); } }
        public string OverviewMessageBoxErrorCaption { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxErrorCaption"); } }
        public string OverviewMessageBoxMailsDeletedNotification { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxMailsDeletedNotification"); } }
        public string OverviewMessageBoxDirectoryNotEmpty { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxDirectoryNotEmpty"); } }
        public string OverviewMessageBoxDeleteFileConfirmationTemplate { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxDeleteFileConfirmationTemplate"); } }
        public string OverviewMessageBoxInvalidAdpsDirectory { get { return _getValueFromTranslationModelOrDefault("OverviewMessageBoxInvalidAdpsDirectory"); } }

        public string MailsFilterFoundCityTemplate { get { return _getValueFromTranslationModelOrDefault("MailsFilterFoundCityTemplate"); } }
        public string MailsFilterNotFoundCityMessage { get { return _getValueFromTranslationModelOrDefault("MailsFilterNotFoundCityMessage"); } }
        public string MailsFilterEnableCheckBoxText { get { return _getValueFromTranslationModelOrDefault("MailsFilterEnableCheckBoxText"); } }
        public string MailsFilterNameCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterNameCaption"); } }
        public string MailsFilterAdditionalNotesCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterAdditionalNotesCaption"); } }
        public string MailsFilterInlineMessageCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterInlineMessageCaption"); } }
        public string MailsFilterHashsumCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterHashsumCaption"); } }
        public string MailsFilterMaxAttachmentSizeCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterMaxAttachmentSizeCaption"); } }
        public string MailsFilterCoordinatesCaptionCoordinatesPart { get { return _getValueFromTranslationModelOrDefault("MailsFilterCoordinatesCaptionCoordinatesPart"); } }
        public string MailsFilterCoordinatesCaptionRadiusPart { get { return _getValueFromTranslationModelOrDefault("MailsFilterCoordinatesCaptionRadiusPart"); } }
        public string MailsFilterDateFromCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterDateFromCaption"); } }
        public string MailsFilterDateToCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterDateToCaption"); } }
        public string MailsFilterDampingDistanceCaption { get { return _getValueFromTranslationModelOrDefault("MailsFilterDampingDistanceCaption"); } }
        public string MailsFilterFindNearestCityMenuHeader { get { return _getValueFromTranslationModelOrDefault("MailsFilterFindNearestCityMenuHeader"); } }
        public string MailsFilterSearchModeNew { get { return _getValueFromTranslationModelOrDefault("MailsFilterSearchModeNew"); } }
        public string MailsFilterSearchModeRefine { get { return _getValueFromTranslationModelOrDefault("MailsFilterSearchModeRefine"); } }
        public string MailsFilterSearchModeUnion { get { return _getValueFromTranslationModelOrDefault("MailsFilterSearchModeUnion"); } }
        public string MailsFilterCancelButtonText { get { return _getValueFromTranslationModelOrDefault("MailsFilterCancelButtonText"); } }
        public string MailsFilterSearchButtonText { get { return _getValueFromTranslationModelOrDefault("MailsFilterSearchButtonText"); } }
        public string MailsFilterCoordinatesTooltip { get { return _getValueFromTranslationModelOrDefault("MailsFilterCoordinatesTooltip"); } }
        public string MailsFilterMaxAttachmentSizeTooltip { get { return _getValueFromTranslationModelOrDefault("MailsFilterMaxAttachmentSizeTooltip"); } }
        public string MailsFilterPopulationTooltip { get { return _getValueFromTranslationModelOrDefault("MailsFilterPopulationTooltip"); } }

        public string MailTotalBytes { get { return _getValueFromTranslationModelOrDefault("MailTotalBytes"); } }
        public string MailNameHeader { get { return _getValueFromTranslationModelOrDefault("MailNameHeader"); } }
        public string MailAdditionalNotesHeader { get { return _getValueFromTranslationModelOrDefault("MailAdditionalNotesHeader"); } }
        public string MailInlineMessageHeader { get { return _getValueFromTranslationModelOrDefault("MailInlineMessageHeader"); } }
        public string MailCoordinatesListHeader { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesListHeader"); } }
        public string MailCoordinatesListTooltipLatitudePart { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesListTooltipLatitudePart"); } }
        public string MailCoordinatesListTooltipLongitudePart { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesListTooltipLongitudePart"); } }
        public string MailCoordinatesTextBoxTooltip { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesTextBoxTooltip"); } }
        public string MailCoordinatesAddButtonText { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesAddButtonText"); } }
        public string MailCoordinatesDeleteButtonText { get { return _getValueFromTranslationModelOrDefault("MailCoordinatesDeleteButtonText"); } }
        public string MailAttachmentsHeader { get { return _getValueFromTranslationModelOrDefault("MailAttachmentsHeader"); } }
        public string MailAttachmentTooltipFilenamePart { get { return _getValueFromTranslationModelOrDefault("MailAttachmentTooltipFilenamePart"); } }
        public string MailAttachmentTooltipSizeBytesPart { get { return _getValueFromTranslationModelOrDefault("MailAttachmentTooltipSizeBytesPart"); } }
        public string MailAttachmentTooltipHashsumPart { get { return _getValueFromTranslationModelOrDefault("MailAttachmentTooltipHashsumPart"); } }
        public string MailAttachmentAddButtonText { get { return _getValueFromTranslationModelOrDefault("MailAttachmentAddButtonText"); } }
        public string MailAttachmentDeleteButtonText { get { return _getValueFromTranslationModelOrDefault("MailAttachmentDeleteButtonText"); } }
        public string MailCreationDateHeader { get { return _getValueFromTranslationModelOrDefault("MailCreationDateHeader"); } }
        public string MailCreationDateNowCheckBoxText { get { return _getValueFromTranslationModelOrDefault("MailCreationDateNowCheckBoxText"); } }
        public string MailCloseButtonText { get { return _getValueFromTranslationModelOrDefault("MailCloseButtonText"); } }
        public string MailSaveButtonText { get { return _getValueFromTranslationModelOrDefault("MailSaveButtonText"); } }
        public string MailExportToFolderButtonText { get { return _getValueFromTranslationModelOrDefault("MailExportToFolderButtonText"); } }
        public string MailUnlockButtonText { get { return _getValueFromTranslationModelOrDefault("MailUnlockButtonText"); } }
        public string MailSaveCopyButtonText { get { return _getValueFromTranslationModelOrDefault("MailSaveCopyButtonText"); } }
        public string MailLockButtonText { get { return _getValueFromTranslationModelOrDefault("MailLockButtonText"); } }
        public string MailProgressBarCalculatingHashsum { get { return _getValueFromTranslationModelOrDefault("MailProgressBarCalculatingHashsum"); } }
        public string MailProgressBarPreparingToCopy { get { return _getValueFromTranslationModelOrDefault("MailProgressBarPreparingToCopy"); } }
        public string MailProgressBarCopyingFiles { get { return _getValueFromTranslationModelOrDefault("MailProgressBarCopyingFiles"); } }
        public string MailProgressBarPreparingToSave { get { return _getValueFromTranslationModelOrDefault("MailProgressBarPreparingToSave"); } }
        public string MailProgressBarSavingMessage { get { return _getValueFromTranslationModelOrDefault("MailProgressBarSavingMessage"); } }
        public string MailMessageBoxFolderNotEmpty { get { return _getValueFromTranslationModelOrDefault("MailMessageBoxFolderNotEmpty"); } }
        public string MailMessageBoxOperationFinished { get { return _getValueFromTranslationModelOrDefault("MailMessageBoxOperationFinished"); } }
        public string MailViewWindowTitle { get { return _getValueFromTranslationModelOrDefault("MailViewWindowTitle"); } }
        public string MailEditWindowTitle { get { return _getValueFromTranslationModelOrDefault("MailEditWindowTitle"); } }
        public string MailNewMessageWindowTitle { get { return _getValueFromTranslationModelOrDefault("MailNewMessageWindowTitle"); } }


        public TranslationViewObject(TranslationModel translationModel)
        {
            TranslationModel = translationModel;
        }

        public static TranslationModel LoadTranslationModel(string path)
        {
            var serializer = new XmlSerializer(typeof(TranslationModel));
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var result = serializer.Deserialize(fs);
                return result as TranslationModel;
            }
        }

        public static TranslationModel SaveCopy(string path, TranslationModel originalTranslation)
        {
            var copy = originalTranslation.Clone();
            copy.Key = Guid.NewGuid().ToString();
            var serializer = new XmlSerializer(typeof(TranslationModel));
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, copy);
            }
            return copy;
        }

        public static string FindTranslationPathByKey(string key)
        {
            for (var index = 0;
                 index < SettingsManager.SettingsManager.Settings.ExternalTranslationKeys.Length;
                 index++)
            {
                if (SettingsManager.SettingsManager.Settings.ExternalTranslationKeys[index] == key)
                {
                    return SettingsManager.SettingsManager.Settings.ExternalTranslationPaths[index];
                }
            }

            return null;
        }

        public static TranslationViewObject LoadFromSettings()
        {
            var translationModel = TranslationModel.GetDefaultEnglish();
            var mode = SettingsManager.SettingsManager.Settings.TranslationLoadingMode;
            var key = SettingsManager.SettingsManager.Settings.TranslationKey;

            var foundKey = false;

            if (mode == TranslationLoadingMode.Embedded)
            {
                foreach (var defaultTranslation in TranslationModel.GetDefaultTranslations())
                {
                    if (defaultTranslation.Key == key)
                    {
                        translationModel = defaultTranslation;
                        foundKey = true;
                        break;
                    }
                }
            }
            else if (mode == TranslationLoadingMode.ExternalFile)
            {
                int index;
                for (index = 0;
                     index < SettingsManager.SettingsManager.Settings.ExternalTranslationKeys.Length;
                     index++)
                {
                    if (SettingsManager.SettingsManager.Settings.ExternalTranslationKeys[index] == key)
                    {
                        foundKey = true;
                        break;
                    }
                }

                if (foundKey)
                {
                    var translationFilePath = SettingsManager.SettingsManager.Settings.ExternalTranslationPaths[index];
                    try
                    {
                        translationModel = LoadTranslationModel(translationFilePath);
                        var expectedKey = translationModel.Key;
                        var expectedTitle = translationModel.Title;
                        SettingsManager.SettingsManager.Settings.CorrectTranslation(translationFilePath,
                                                                                    expectedKey,
                                                                                    expectedTitle);
                    }
                    catch (Exception)
                    {
                        foundKey = false;
                    }
                }
            }

            if (!foundKey)
            {
                SettingsManager.SettingsManager.Settings.RemoveExternalTranslation(key);
                SettingsManager.SettingsManager.Settings.TranslationKey = translationModel.Key;
                SettingsManager.SettingsManager.Settings.TranslationLoadingMode = TranslationLoadingMode.Embedded;
            }

            return new TranslationViewObject(translationModel);
        }


    }
}
