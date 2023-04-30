using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class DataManager : IService, IDataManager
    {
        private IAppStateManager _appStateManager;
        private ILocalizationManager _localizationManager;


        public event Action EndLoadCache;

        private Dictionary<Enumerators.CacheDataType, string> _cacheDataPathes;

        public UserLocalData CachedUserLocalData { get; set; }

        private Dictionary<Enumerators.SpreadsheetDataType, SpreadsheetInfo> _spreadsheetsInfo;

        public List<RecordItem> UserLocalRecords { get; set; }
        public List<RecordItem> GlobalRecords { get; set; }

        public DataManager()
        {
            CachedUserLocalData = new UserLocalData();
            UserLocalRecords = new List<RecordItem>();
            GlobalRecords = new List<RecordItem>();
        }

        public void Dispose()
        {
            SaveAllCache();
        }

        public void Init()
        {
            _appStateManager = GameClient.Get<IAppStateManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            FillSpreadsheetsInfo();
            FillCacheDataPathes();
        }

        public async void StartLoadCache()
        {

            int count = Enum.GetNames(typeof(Enumerators.CacheDataType)).Length;
            for (int i = 0; i < count; i++)
                LoadCachedData((Enumerators.CacheDataType)i);

            await StartLoadSpreadsheetsData();

            _appStateManager.ChangeAppState(Enumerators.AppState.MAIN_MENU);

            EndLoadCache?.Invoke();
        }

        public void Update()
        {

        }

        public void SaveAllCache()
        {
            int count = Enum.GetNames(typeof(Enumerators.CacheDataType)).Length;
            for (int i = 0; i < count; i++)
                SaveCache((Enumerators.CacheDataType)i);
        }


        public Sprite GetSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);
        }


        public SpreadsheetInfo GetSpreadsheetByType(Enumerators.SpreadsheetDataType type)
        {
            return _spreadsheetsInfo[type];
        }

        public void SaveCache(Enumerators.CacheDataType type)
        {
            switch (type)
            {
                case Enumerators.CacheDataType.USER_LOCAL_DATA:
                    {
                        Debug.Log(File.Exists(_cacheDataPathes[type]));
                        if (!File.Exists(_cacheDataPathes[type]))
                            File.Create(_cacheDataPathes[type]).Close();

                        File.WriteAllText(_cacheDataPathes[type], JsonConvert.SerializeObject(CachedUserLocalData));
                    }
                    break;
                case Enumerators.CacheDataType.USER_RECORDS_DATA:
                    {
                        if (!File.Exists(_cacheDataPathes[type]))
                            File.Create(_cacheDataPathes[type]).Close();

                        File.WriteAllText(_cacheDataPathes[type], JsonConvert.SerializeObject(UserLocalRecords));
                    }
                    break;
                default: break;
            }
        }

        private void LoadCachedData(Enumerators.CacheDataType type)
        {
            switch (type)
            {
                case Enumerators.CacheDataType.USER_LOCAL_DATA:
                    {
                        if (File.Exists(_cacheDataPathes[type]))
                            CachedUserLocalData = DeserializeObjectFromPath<UserLocalData>(_cacheDataPathes[type]);
                        if (CachedUserLocalData.PlayerCharacteristicsData.Count <= 0)
                        {
                            CachedUserLocalData.SetCharecteristicData();
                        }
                    }
                    break;
                case Enumerators.CacheDataType.USER_RECORDS_DATA:
                    {
                        if (File.Exists(_cacheDataPathes[type]))
                            UserLocalRecords = DeserializeObjectFromPath<List<RecordItem>>(_cacheDataPathes[type]);
                    }
                    break;
                default: break;
            }
        }

        public void AddRecord(RecordItem item)
        {
            if (UserLocalRecords.Count >= Constants.MAX_RECORDS - 1)
            {
                RecordItem minimalRecord = UserLocalRecords[0];
                foreach (var record in UserLocalRecords)
                {
                    if (minimalRecord.Score <= record.Score)
                    {
                        minimalRecord = record;
                        break;
                    }
                }
                if (minimalRecord.Score > item.Score)
                {
                    return;
                }
                UserLocalRecords.Remove(minimalRecord);
            }

            UserLocalRecords.Add(item);
            UserLocalRecords.Sort(delegate (RecordItem item1, RecordItem item2)
            {
                return item2.Score.CompareTo(item1.Score);
            });
        }


        private void FillCacheDataPathes()
        {
            _cacheDataPathes = new Dictionary<Enumerators.CacheDataType, string>();
            _cacheDataPathes.Add(Enumerators.CacheDataType.USER_LOCAL_DATA, Path.Combine(Application.persistentDataPath, Constants.LOCAL_USER_DATA_FILE_PATH));
            _cacheDataPathes.Add(Enumerators.CacheDataType.USER_RECORDS_DATA, Path.Combine(Application.persistentDataPath, Constants.LOCAL_USER_RECORDS));
        }

        public T DeserializeObjectFromPath<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        private async Task StartLoadSpreadsheetsData()
        {
            foreach (var item in _spreadsheetsInfo)
            {
                await item.Value.LoadData();
            }
        }

        private void FillSpreadsheetsInfo()
        {
            _spreadsheetsInfo = new Dictionary<Enumerators.SpreadsheetDataType, SpreadsheetInfo>();

            if (_localizationManager.LocalizationData.refreshLocalizationAtStart)
                _spreadsheetsInfo.Add(Enumerators.SpreadsheetDataType.Localization, new SpreadsheetInfo(_localizationManager.LocalizationData.localizationGoogleSpreadsheet));
        }
    }
}