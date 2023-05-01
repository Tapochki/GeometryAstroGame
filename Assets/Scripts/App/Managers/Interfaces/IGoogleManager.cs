using System;
using System.Collections.Generic;
using static TandC.RunIfYouWantToLive.LeaderBoardPage;

namespace TandC.RunIfYouWantToLive 
{
    public interface IGoogleManager
    {
        Action<bool> PlayerAuthentificate { get; set; }
        bool IsAuthenticated { get; set; }
        string DisplayedName { get; set; }
        void Authenticate();
        void LoadLeaderboardData(Action<List<GlobalRecordItem>> callback);
        void LoadPlayerScore(Action<GlobalRecordItem> callback);
        void SaveDataToCloud(string fileName, byte[] data, Action<bool> callback);
        void LoadDataFromCloud(string fileName, Action<byte[]> callback);
        void SavePlayerScore(string leaderboardId, long score);
    }
}

