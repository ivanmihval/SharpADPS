using System.Collections.Generic;

namespace WPFSharpADPS.Models.Translation
{
    public class TranslationModel
    {
        public string Key;
        public string Title;

        public string OverviewFilesMenuTitle;
        public string OverviewNewRepositoryTitle;
        public string OverviewLoadRepositoryTitle;
        public string OverviewNewMailTitle;
        public string OverviewCopyToRepositoryTitle;
        public string OverviewDeleteFromRepositoryTitle;
        public string OverviewRecentRepositoriesTitle;
        public string OverviewExitTitle;

        public string OverviewSelectionMenuTitle;
        public string OverviewRemoveSelectedItemsTitle;
        public string OverviewLeaveSelectedItemsTitle;

        public string OverviewFiltersMenuTitle;
        public string OverviewResetFiltersTitle;
        public string OverviewApplyFiltersTitle;

        public string OverviewSettingsMenuTitle;

        public string OverviewHelpMenuTitle;
        public string OverviewDocumentationTitle;
        public string OverviewAboutTitle;

        public string OverviewDataGridDateHeader;
        public string OverviewDataGridCoordinatesHeader;
        public string OverviewDataGridNameHeader;
        public string OverviewDataGridAttachmentsHeader;
        public string OverviewDataGridAdditionalNotesHeader;
        public string OverviewDataGridInlineMessageHeader;

        public string OverviewDataGridContextMenuOpen;

        public string OverviewCloseRepoButtonText;
        public string OverviewShowAllMailsButtonText;
        public string OverviewIncreaseRowsPerPageTooltip;
        public string OverviewDecreaseRowsPerPageTooltip;
        public string OverviewChangeRowsPerPageTooltipCurrentValue;
        public string OverviewTotalMessagesTextBlock;
        public string OverviewNextPageTooltip;
        public string OverviewPreviousPageTooltip;

        public string OverviewNoRepoLoadedTitle;

        public string OverviewDataGridDaysAgoTemplate;
        public string OverviewDataGridManyCoordinatesTemplate;
        public string OverviewDataGridNoCoordinates;
        public string OverviewDataGridManyFilesTemplate;
        public string OverviewDataGridNoFiles;

        public string OverviewProgressBarFilteringMessages;
        public string OverviewProgressBarCopyEstimation;
        public string OverviewProgressBarCopying;
        public string OverviewProgressBarScanningTargetFiles;
        public string OverviewProgressBarScanningAllFiles;
        public string OverviewProgressBarDeleteFiles;

        public string OverviewMessageBoxConfirmationCaption;
        public string OverviewMessageBoxNotificationCaption;
        public string OverviewMessageBoxErrorCaption;
        public string OverviewMessageBoxMailsDeletedNotification;
        public string OverviewMessageBoxDirectoryNotEmpty;
        public string OverviewMessageBoxDeleteFileConfirmationTemplate;
        public string OverviewMessageBoxInvalidAdpsDirectory;

        public string MailsFilterFoundCityTemplate;
        public string MailsFilterNotFoundCityMessage;
        public string MailsFilterEnableCheckBoxText;
        public string MailsFilterNameCaption;
        public string MailsFilterAdditionalNotesCaption;
        public string MailsFilterInlineMessageCaption;
        public string MailsFilterHashsumCaption;
        public string MailsFilterMaxAttachmentSizeCaption;
        public string MailsFilterCoordinatesCaptionCoordinatesPart;
        public string MailsFilterCoordinatesCaptionRadiusPart;
        public string MailsFilterDateFromCaption;
        public string MailsFilterDateToCaption;
        public string MailsFilterDampingDistanceCaption;
        public string MailsFilterFindNearestCityMenuHeader;
        public string MailsFilterSearchModeNew;
        public string MailsFilterSearchModeRefine;
        public string MailsFilterSearchModeUnion;
        public string MailsFilterCancelButtonText;
        public string MailsFilterSearchButtonText;
        public string MailsFilterCoordinatesTooltip;
        public string MailsFilterMaxAttachmentSizeTooltip;
        public string MailsFilterPopulationTooltip;

        public string MailTotalBytes;
        public string MailNameHeader;
        public string MailAdditionalNotesHeader;
        public string MailInlineMessageHeader;
        public string MailCoordinatesListHeader;
        public string MailCoordinatesListTooltipLatitudePart;
        public string MailCoordinatesListTooltipLongitudePart;
        public string MailCoordinatesTextBoxTooltip;
        public string MailCoordinatesAddButtonText;
        public string MailCoordinatesDeleteButtonText;
        public string MailAttachmentsHeader;
        public string MailAttachmentTooltipFilenamePart;
        public string MailAttachmentTooltipSizeBytesPart;
        public string MailAttachmentTooltipHashsumPart;
        public string MailAttachmentAddButtonText;
        public string MailAttachmentDeleteButtonText;
        public string MailCreationDateHeader;
        public string MailCreationDateNowCheckBoxText;
        public string MailCloseButtonText;
        public string MailSaveButtonText;
        public string MailExportToFolderButtonText;
        public string MailUnlockButtonText;
        public string MailSaveCopyButtonText;
        public string MailLockButtonText;
        public string MailProgressBarCalculatingHashsum;
        public string MailProgressBarPreparingToCopy;
        public string MailProgressBarCopyingFiles;
        public string MailProgressBarPreparingToSave;
        public string MailProgressBarSavingMessage;
        public string MailMessageBoxFolderNotEmpty;
        public string MailMessageBoxOperationFinished;
        public string MailViewWindowTitle;
        public string MailEditWindowTitle;
        public string MailNewMessageWindowTitle;


        public static TranslationModel GetDefaultEnglish()
        {
            return new TranslationModel
                {
                    Key = "en",
                    Title = "English",
                    OverviewFilesMenuTitle = "Files",
                    OverviewNewRepositoryTitle = "New repository...",
                    OverviewLoadRepositoryTitle = "Load repositiry...",
                    OverviewNewMailTitle = "New mail...",
                    OverviewCopyToRepositoryTitle = "Save filtered mails to existing repository...",
                    OverviewDeleteFromRepositoryTitle = "Delete filtered mails from the repository...",
                    OverviewRecentRepositoriesTitle = "Recent repositories",
                    OverviewExitTitle = "Exit",
                    OverviewSelectionMenuTitle = "Selection",
                    OverviewRemoveSelectedItemsTitle = "Remove selected mails from the filtered mails list",
                    OverviewLeaveSelectedItemsTitle = "Leave selected mails in the filtered mails list",
                    OverviewFiltersMenuTitle = "Filter",
                    OverviewResetFiltersTitle = "Reset filters",
                    OverviewApplyFiltersTitle = "Apply filters...",
                    OverviewSettingsMenuTitle = "Settings",
                    OverviewHelpMenuTitle = "Help",
                    OverviewDocumentationTitle = "Documentaion",
                    OverviewAboutTitle = "About...",
                    OverviewDataGridDateHeader = "Date",
                    OverviewDataGridCoordinatesHeader = "Coordinates",
                    OverviewDataGridNameHeader = "Name",
                    OverviewDataGridAttachmentsHeader = "Attachments",
                    OverviewDataGridAdditionalNotesHeader = "AdditionalNotes",
                    OverviewDataGridInlineMessageHeader = "InlineMessage",
                    OverviewDataGridContextMenuOpen = "Open",
                    OverviewCloseRepoButtonText = "Close",
                    OverviewShowAllMailsButtonText = "Reload repo",
                    OverviewIncreaseRowsPerPageTooltip = "Increase rows per page",
                    OverviewDecreaseRowsPerPageTooltip = "Decrease rows per page",
                    OverviewChangeRowsPerPageTooltipCurrentValue = "current value",
                    OverviewTotalMessagesTextBlock = "Total",
                    OverviewNextPageTooltip = "Next page",
                    OverviewPreviousPageTooltip = "Previous page",
                    OverviewNoRepoLoadedTitle = "No repo loaded",
                    OverviewDataGridDaysAgoTemplate = "{0} days ago",
                    OverviewDataGridNoCoordinates = "No coordinates",
                    OverviewDataGridManyCoordinatesTemplate = "{0} coordinates",
                    OverviewDataGridNoFiles = "No files",
                    OverviewDataGridManyFilesTemplate = "{0} files",
                    OverviewProgressBarFilteringMessages = "Filtering messages...",
                    OverviewProgressBarCopyEstimation = "Estimation...",
                    OverviewProgressBarCopying = "Copying...",
                    OverviewProgressBarScanningTargetFiles = "Scanning target files...",
                    OverviewProgressBarScanningAllFiles = "Scanning all files...",
                    OverviewProgressBarDeleteFiles = "Delete files...",
                    OverviewMessageBoxConfirmationCaption = "Confirmation",
                    OverviewMessageBoxNotificationCaption = "Notification",
                    OverviewMessageBoxErrorCaption = "Error",
                    OverviewMessageBoxMailsDeletedNotification = "The filtered mails have been successfully deleted",
                    OverviewMessageBoxDirectoryNotEmpty = "The chosen directory is not empty",
                    OverviewMessageBoxDeleteFileConfirmationTemplate =
                        "Do you really want to delete {0} message files with associated attachments?",
                    OverviewMessageBoxInvalidAdpsDirectory = "The chosen directory is not like a valid ADPS repo",
                    //
                    MailsFilterFoundCityTemplate = "Found city {0}, {1}. Population {2}.",
                    MailsFilterNotFoundCityMessage = "Could not find city.",
                    MailsFilterEnableCheckBoxText = "Enable",
                    MailsFilterNameCaption = "Name:",
                    MailsFilterAdditionalNotesCaption = "Additional Notes:",
                    MailsFilterInlineMessageCaption = "Inline Message:",
                    MailsFilterHashsumCaption = "Hashsum:",
                    MailsFilterMaxAttachmentSizeCaption = "Max attachment size (bytes):",
                    MailsFilterCoordinatesCaptionCoordinatesPart = "Coordinates",
                    MailsFilterCoordinatesCaptionRadiusPart = "radius",
                    MailsFilterDateFromCaption = "Date From:",
                    MailsFilterDateToCaption = "Date To:",
                    MailsFilterDampingDistanceCaption = "Population of city (Damping Distance Filter):",
                    MailsFilterFindNearestCityMenuHeader = "Find nearest city",
                    MailsFilterSearchModeNew = "New search",
                    MailsFilterSearchModeRefine = "Refine current search",
                    MailsFilterSearchModeUnion = "Add results to current search",
                    MailsFilterCancelButtonText = "Cancel",
                    MailsFilterSearchButtonText = "Search",
                    MailsFilterCoordinatesTooltip = "Format: 'lat,lon'. Examples: '-1.234, 56.78', '55.44, 66.77'",
                    MailsFilterMaxAttachmentSizeTooltip = "Enter a correct uint_64 value",
                    MailsFilterPopulationTooltip = "Enter an integer value",
                    //
                    MailTotalBytes = "Total bytes",
                    MailNameHeader = "Name",
                    MailAdditionalNotesHeader = "Additional Notes",
                    MailInlineMessageHeader = "Inline Message",
                    MailCoordinatesListHeader = "Coordinates List",
                    MailCoordinatesListTooltipLatitudePart = "Latitude",
                    MailCoordinatesListTooltipLongitudePart = "Longitude",
                    MailCoordinatesTextBoxTooltip = "Format: lat,lon. For example: '-1.2345, 6.7890'",
                    MailCoordinatesAddButtonText = "Add",
                    MailCoordinatesDeleteButtonText = "Delete",
                    MailAttachmentsHeader = "Attachments",
                    MailAttachmentTooltipFilenamePart = "Filename",
                    MailAttachmentTooltipSizeBytesPart = "Size (bytes)",
                    MailAttachmentTooltipHashsumPart = "Hashsum",
                    MailAttachmentAddButtonText = "Add",
                    MailAttachmentDeleteButtonText = "Delete",
                    MailCreationDateHeader = "Creation Date",
                    MailCreationDateNowCheckBoxText = "NOW",
                    MailCloseButtonText = "Close",
                    MailSaveButtonText = "Save",
                    MailExportToFolderButtonText = "Export To Folder",
                    MailUnlockButtonText = "Unlock",
                    MailSaveCopyButtonText = "Save Copy",
                    MailLockButtonText = "Lock",
                    MailProgressBarCalculatingHashsum = "Calculating hashsum...",
                    MailProgressBarPreparingToCopy = "Preparing to copy...",
                    MailProgressBarCopyingFiles = "Copying files...",
                    MailProgressBarPreparingToSave = "Preparing the message to save...",
                    MailProgressBarSavingMessage = "Saving the message to the repository...",
                    MailMessageBoxFolderNotEmpty = "The folder should be empty",
                    MailMessageBoxOperationFinished = "The operation is finished successfully!",
                    MailViewWindowTitle = "View",
                    MailEditWindowTitle = "Edit",
                    MailNewMessageWindowTitle = "New message",
                };
        }

        public static TranslationModel GetDefaultRussian()
        {
            return new TranslationModel
                {
                    Key = "ru",
                    Title = "Русский",
                    OverviewFilesMenuTitle = "Файлы",
                    OverviewNewRepositoryTitle = "Новый репозиторий...",
                    OverviewLoadRepositoryTitle = "Открыть репозиторий...",
                    OverviewNewMailTitle = "Новое письмо...",
                    OverviewCopyToRepositoryTitle = "Сохранить фильтрованные письма в существующий репозиторий...",
                    OverviewDeleteFromRepositoryTitle = "Удалить фильтрованные письма из репозитория...",
                    OverviewRecentRepositoriesTitle = "Недавние репозитории",
                    OverviewExitTitle = "Выход",
                    OverviewSelectionMenuTitle = "Выделение",
                    OverviewRemoveSelectedItemsTitle = "Удалить выделенное из списка фильтрованных писем",
                    OverviewLeaveSelectedItemsTitle = "Удалить всё, кроме выделенного, из списка фильтрованных писем",
                    OverviewFiltersMenuTitle = "Фильтр",
                    OverviewResetFiltersTitle = "Сбросить фильтр",
                    OverviewApplyFiltersTitle = "Применить фильтр...",
                    OverviewSettingsMenuTitle = "Настройки",
                    OverviewHelpMenuTitle = "Помощь",
                    OverviewDocumentationTitle = "Документация",
                    OverviewAboutTitle = "О программе...",
                    OverviewDataGridDateHeader = "Дата",
                    OverviewDataGridCoordinatesHeader = "Координаты",
                    OverviewDataGridNameHeader = "Имя",
                    OverviewDataGridAttachmentsHeader = "Вложения",
                    OverviewDataGridAdditionalNotesHeader = "Примечания",
                    OverviewDataGridInlineMessageHeader = "Сообщение",
                    OverviewDataGridContextMenuOpen = "Открыть",
                    OverviewCloseRepoButtonText = "Закрыть",
                    OverviewShowAllMailsButtonText = "Обновить репозиторий",
                    OverviewIncreaseRowsPerPageTooltip = "Увеличить число строк на страницу",
                    OverviewDecreaseRowsPerPageTooltip = "Уменьшить число строк на страницу",
                    OverviewChangeRowsPerPageTooltipCurrentValue = "текущее значение",
                    OverviewTotalMessagesTextBlock = "Всего",
                    OverviewNextPageTooltip = "Следующая страница",
                    OverviewPreviousPageTooltip = "Предыдущая страница",
                    OverviewNoRepoLoadedTitle = "Репозиторий не загружен",
                    OverviewDataGridDaysAgoTemplate = "{0} дня(-ей) назад",
                    OverviewDataGridNoCoordinates = "Координаты не указаны",
                    OverviewDataGridManyCoordinatesTemplate = "{0} координат(-ы)",
                    OverviewDataGridNoFiles = "Нет вложений",
                    OverviewDataGridManyFilesTemplate = "{0} файл(-ов)",
                    OverviewProgressBarFilteringMessages = "Фильтрация сообщений...",
                    OverviewProgressBarCopyEstimation = "Оценивание...",
                    OverviewProgressBarCopying = "Копирование...",
                    OverviewProgressBarScanningTargetFiles = "Сканирование файлов на удаление...",
                    OverviewProgressBarScanningAllFiles = "Сканирование всех файлов...",
                    OverviewProgressBarDeleteFiles = "Удаление файлов...",
                    OverviewMessageBoxConfirmationCaption = "Подтверждение",
                    OverviewMessageBoxNotificationCaption = "Уведомление",
                    OverviewMessageBoxErrorCaption = "Ошибка",
                    OverviewMessageBoxMailsDeletedNotification = "Фильтрованные письма удалены.",
                    OverviewMessageBoxDirectoryNotEmpty = "Выбранная директория не является пустой.",
                    OverviewMessageBoxDeleteFileConfirmationTemplate =
                        "Вы действительно хотите удалить {0} сообщений вместе с прикреплёнными вложениями?",
                    OverviewMessageBoxInvalidAdpsDirectory = "Выбранная директория не является репозиторием ADPS",
                    MailsFilterFoundCityTemplate = "Найден город: {0}, Страна: {1}. Население: {2} человек.",
                    MailsFilterNotFoundCityMessage = "Не получилось найти город по введённым координатам.",
                    MailsFilterEnableCheckBoxText = "Применить",
                    MailsFilterNameCaption = "Имя:",
                    MailsFilterAdditionalNotesCaption = "Примечания:",
                    MailsFilterInlineMessageCaption = "Сообщение:",
                    MailsFilterHashsumCaption = "Хешсумма:",
                    MailsFilterMaxAttachmentSizeCaption = "Максимальный размер вложения (байт):",
                    MailsFilterCoordinatesCaptionCoordinatesPart = "Координаты",
                    MailsFilterCoordinatesCaptionRadiusPart = "радиус",
                    MailsFilterDateFromCaption = "Начальная дата:",
                    MailsFilterDateToCaption = "Конечная дата:",
                    MailsFilterDampingDistanceCaption = "Население города (фильтр убывающего расстояния):",
                    MailsFilterFindNearestCityMenuHeader = "Найти ближайший город",
                    MailsFilterSearchModeNew = "Новый поиск",
                    MailsFilterSearchModeRefine = "Искать в найденном",
                    MailsFilterSearchModeUnion = "Объединить результаты поиска",
                    MailsFilterCancelButtonText = "Отмена",
                    MailsFilterSearchButtonText = "Поиск",
                    MailsFilterCoordinatesTooltip = "Формат: 'широта, долгота'. Например, '-1.234, 56.78', '55.44, 66.77'.",
                    MailsFilterMaxAttachmentSizeTooltip = "Введите корректное uint_64 значение",
                    MailsFilterPopulationTooltip = "Введите целое число",
                    //
                    MailTotalBytes = "Всего байт",
                    MailNameHeader = "Имя",
                    MailAdditionalNotesHeader = "Примечания",
                    MailInlineMessageHeader = "Сообщение",
                    MailCoordinatesListHeader = "Список координат",
                    MailCoordinatesListTooltipLatitudePart = "Широта",
                    MailCoordinatesListTooltipLongitudePart = "Долгота",
                    MailCoordinatesTextBoxTooltip = "Формат: 'широта, долгота'. Например, '-1.234, 56.78', '55.44, 66.77'.",
                    MailCoordinatesAddButtonText = "Добавить",
                    MailCoordinatesDeleteButtonText = "Удалить",
                    MailAttachmentsHeader = "Вложения",
                    MailAttachmentTooltipFilenamePart = "Имя файла",
                    MailAttachmentTooltipSizeBytesPart = "Размер (байт)",
                    MailAttachmentTooltipHashsumPart = "Хешсумма",
                    MailAttachmentAddButtonText = "Добавить",
                    MailAttachmentDeleteButtonText = "Удалить",
                    MailCreationDateHeader = "Дата создания",
                    MailCreationDateNowCheckBoxText = "СЕЙЧАС",
                    MailCloseButtonText = "Закрыть",
                    MailSaveButtonText = "Сохранить",
                    MailExportToFolderButtonText = "Экспортировать в папку",
                    MailUnlockButtonText = "Редактировать",
                    MailSaveCopyButtonText = "Сохранить копию",
                    MailLockButtonText = "Блокировка",
                    MailProgressBarCalculatingHashsum = "Расчёт хешсуммы...",
                    MailProgressBarPreparingToCopy = "Подготовка к копированию...",
                    MailProgressBarCopyingFiles = "Копирование...",
                    MailProgressBarPreparingToSave = "Подготовка к сохранению...",
                    MailProgressBarSavingMessage = "Сохранение в репозиторий...",
                    MailMessageBoxFolderNotEmpty = "Папка должна быть пустой.",
                    MailMessageBoxOperationFinished = "Операция завершена успешно!",
                    MailViewWindowTitle = "Просмотр",
                    MailEditWindowTitle = "Редактирование",
                    MailNewMessageWindowTitle = "Новое сообщение",
                };
        }

        public static List<TranslationModel> GetDefaultTranslations()
        {
            return new List<TranslationModel> {GetDefaultEnglish(), GetDefaultRussian()};
        }

        public TranslationModel Clone()
        {
            return (TranslationModel) this.MemberwiseClone();
        }
    }
}
