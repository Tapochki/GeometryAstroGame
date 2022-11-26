using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IDataManager
    {
        UserLocalData CachedUserLocalData { get; set; }
        List<RecordItem> UserLocalRecords { get; set; }
        void StartLoadCache();
        void SaveAllCache();
        void SaveCache(Enumerators.CacheDataType type);
        void AddRecord(RecordItem item);
        Sprite GetSpriteFromTexture(Texture2D texture);
        event Action EndLoadCache;
    }
}