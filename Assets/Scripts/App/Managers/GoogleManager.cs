using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static TandC.RunIfYouWantToLive.LeaderBoardPage;

namespace TandC.RunIfYouWantToLive
{
    public class GoogleManager : IService
    {
        public Action<bool> PlayerAuthentificate;

        private const string _leaderBoardId = "";

        private bool isAuthenticated;

        public string DisplayedName;
        public void Dispose()
        {

        }

        public void Init()
        {
            Authenticate();
        }

        public void Update()
        {

        }

        public void Authenticate()
        {
            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate(success =>
            {
                isAuthenticated = success;
                DisplayedName = Social.localUser.userName;
                PlayerAuthentificate?.Invoke(success);
            });
        }

        public void LoadLeaderboardData(Action<List<GlobalRecordItem>> callback)
        {
            if (isAuthenticated)
            {
                PlayGamesPlatform.Instance.LoadScores(_leaderBoardId, LeaderboardStart.TopScores, 50, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (data) =>
                {
                    if (data.Valid && data.Scores != null && data.Scores.Length > 0)
                    {
                        List<GlobalRecordItem> scores = new List<GlobalRecordItem>();
                        Debug.LogError($"rank {data.PlayerScore.rank} name {data.PlayerScore.userID} value {data.PlayerScore.value}");
                        foreach (IScore score in data.Scores)
                        {
                            scores.Add(new GlobalRecordItem() {Id = score.rank, Name = score.userID, Score = score.value});
                        }
                        callback(scores);
                    }
                    else
                    {
                        callback(null);
                    }
                });
            }
            else
            {
                callback(null);
            }
        }

        public void LoadPlayerScore(Action<GlobalRecordItem> callback)
        {
            if (isAuthenticated)
            {
                PlayGamesPlatform.Instance.LoadScores(
                    _leaderBoardId,
                    LeaderboardStart.PlayerCentered,
                    1,
                    LeaderboardCollection.Public,
                    LeaderboardTimeSpan.AllTime,
                    (LeaderboardScoreData data) =>
                    {
                        if (data.PlayerScore != null)
                        {
                            GlobalRecordItem currentPlayerRecord = new GlobalRecordItem() { Id = data.PlayerScore.rank, Score = data.PlayerScore.value, Name = data.PlayerScore.userID };
                            callback(currentPlayerRecord);
                        }
                        else
                        {
                            Debug.LogError("Unable to load player score data.");
                            callback(null);
                        }
                    }
                );
            }
            else
            {
                Debug.LogError("User is not authenticated with Google Play Games.");
                callback(null);
            }
        }

        public void SaveDataToCloud(string fileName, byte[] data, Action<bool> callback)
        {
            if (isAuthenticated)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseMostRecentlySaved, (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved game at " + DateTime.Now);
                        SavedGameMetadataUpdate updatedMetadata = builder.Build();

                        savedGameClient.CommitUpdate(metadata, updatedMetadata, data, (commitStatus, _) =>
                        {
                            if (commitStatus == SavedGameRequestStatus.Success)
                            {
                                callback(true);
                            }
                            else
                            {
                                callback(false);
                            }
                        });
                    }
                    else
                    {
                        callback(false);
                    }
                });
            }
            else
            {
                callback(false);
            }
        }

        public void LoadDataFromCloud(string fileName, Action<byte[]> callback)
        {
            if (isAuthenticated)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseMostRecentlySaved, (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        savedGameClient.ReadBinaryData(metadata, (dataStatus, data) =>
                        {
                            if (dataStatus == SavedGameRequestStatus.Success)
                            {
                                callback(data);
                            }
                            else
                            {
                                callback(null);
                            }
                        });
                    }
                    else
                    {
                        callback(null);
                    }
                });
            }
            else
            {
                callback(null);
            }
        }

        public void SavePlayerScore(string leaderboardId, long score)
        {
            if (isAuthenticated)
            {
                Social.ReportScore(score, leaderboardId, success =>
                {
                    if (success)
                    {
                        Debug.Log("Score reported successfully");
                    }
                    else
                    {
                        Debug.LogError("Failed to report score");
                    }
                });
            }
        }
    }
}

