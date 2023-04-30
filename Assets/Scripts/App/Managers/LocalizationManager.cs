using System;
using System.Collections.Generic;
using System.Linq;
using TandC.RunIfYouWantToLive.Common;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class LocalizationManager : IService, ILocalizationManager
    {
        public event Action<Enumerators.Language> LanguageWasChangedEvent;

        private IDataManager _dataManager;

        private ILoadObjectsManager _loadObjectsManager;

        private LocalizationData.LocalizationLanguageData _currentLocalizationLanguageData;

        private Dictionary<string, string> _localizationTexts;

        public Dictionary<SystemLanguage, Enumerators.Language> SupportedLanguages { get; private set; }
        public Enumerators.Language CurrentLanguage { get; private set; }
        public Enumerators.Language DefaultLanguage { get; private set; }
        public LocalizationData LocalizationData { get; private set; }

        public void Dispose()
        {
            _dataManager.EndLoadCache -= DataLoadedEventHandler;
        }

        public void Update()
        {
        }

        public void Init()
        {
            _dataManager = GameClient.Get<IDataManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();

            LocalizationData = _loadObjectsManager.GetObjectByPath<LocalizationData>("Data/LocalizationData");

            DefaultLanguage = LocalizationData.defaultLanguage;
            CurrentLanguage = Enumerators.Language.Unknown;

            _localizationTexts = new Dictionary<string, string>();

            _dataManager.EndLoadCache += DataLoadedEventHandler;

            FillLanguages();
        }

        public void SetLanguage(Enumerators.Language language, bool forceUpdate = false)
        {
            if (language == CurrentLanguage && !forceUpdate)
                return;

            _dataManager.CachedUserLocalData.appLanguage = language;

            if (SupportedLanguages.ContainsValue(language))
            {
                CurrentLanguage = language;
                _currentLocalizationLanguageData = LocalizationData.languages.Find(item => item.language == CurrentLanguage);
            }

            LanguageWasChangedEvent?.Invoke(CurrentLanguage);
        }

        public string GetUITranslation(string key)
        {
            if (_currentLocalizationLanguageData == null)
                return key;

            var localizedText = _currentLocalizationLanguageData.localizedTexts.
                Find(item => InternalTools.ReplaceLineBreaks(item.key) == InternalTools.ReplaceLineBreaks(key));

            if (localizedText == null)
                return key;

            return localizedText.value;
        }

        private void DataLoadedEventHandler()
        {
            if (LocalizationData.refreshLocalizationAtStart)
                RefreshLocalizations();
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            if (!SupportedLanguages.ContainsKey(Application.systemLanguage))
            {
                if (_dataManager.CachedUserLocalData.appLanguage == Enumerators.Language.Unknown)
                    SetLanguage(DefaultLanguage, true);
                else
                    SetLanguage(_dataManager.CachedUserLocalData.appLanguage, true);
            }
            else
            {
                if (_dataManager.CachedUserLocalData.appLanguage == Enumerators.Language.Unknown)
                    SetLanguage(SupportedLanguages[Application.systemLanguage], true);
                else
                    SetLanguage(_dataManager.CachedUserLocalData.appLanguage, true);
            }
        }

        private void FillLanguages()
        {
            SupportedLanguages = new Dictionary<SystemLanguage, Enumerators.Language>();

            var supportedLanguages = LocalizationData.languages.Select(item => item.language).ToArray();

            foreach (var item in supportedLanguages)
            {
                if (Enum.TryParse(item.ToString(), out SystemLanguage result))
                {
                    SupportedLanguages.Add(result, item);
                }
                else
                {
                    Debug.Log($"Cannot parse unsupported localziation language: {item}");
                }
            }
        }


        private void RefreshLocalizations()
        {
            var spreadsheet = _dataManager.GetSpreadsheetByType(Enumerators.SpreadsheetDataType.Localization);

            if (spreadsheet == null)
            {
                Debug.Log("Failed to refresh localization. Spreadsheet is null.");
                return;
            }
            else if (!spreadsheet.IsLoaded)
            {
                Debug.Log("Failed to refresh localization. Spreadsheet is not loaded.");
                return;
            }

            var localizationSheetData = spreadsheet.GetObject<LocalizationSheetData>();

            LocalizationData.languages = new List<LocalizationData.LocalizationLanguageData>();

            LocalizationData.LocalizationLanguageData languageData;
            LocalizationData.LocalizationDataInfo dataInfo;
            for (int i = 1; i < Enum.GetNames(typeof(Common.Enumerators.Language)).Length; i++)
            {
                languageData = new LocalizationData.LocalizationLanguageData()
                {
                    language = (Common.Enumerators.Language)i,
                    localizedTexts = new List<LocalizationData.LocalizationDataInfo>()
                };

                foreach (var element in localizationSheetData)
                {
                    dataInfo = new LocalizationData.LocalizationDataInfo()
                    {
                        key = element.Keys
                    };

                    switch (languageData.language)
                    {
                        case Common.Enumerators.Language.Russian:
                            dataInfo.value = element.Russian;
                            break;
                        case Common.Enumerators.Language.Ukrainian:
                            dataInfo.value = element.Ukrainian;
                            break;
                        case Common.Enumerators.Language.English:
                        default:
                            dataInfo.value = element.English;
                            break;
                    }

                    languageData.localizedTexts.Add(dataInfo);
                }

                LocalizationData.languages.Add(languageData);
            }
        }
    }
}