using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    public sealed class ScenesManager : IService, IScenesManager
    {
        private bool _isLoadingScenesAsync = true;
        private bool _isLoadingStarted = false;

        private IAppStateManager _appStateManager;
        private IUIManager _uiManager;

        public static string CurrentSceneName { get; private set; }
        public int SceneLoadingProgress { get; private set; } 

        public bool IsLoadedScene { get; set; }

        public void Dispose()
        {
            MainApp.Instance.OnLevelWasLoadedEvent -= OnLevelWasLoadedHandler;
        }

        public void Init()
        {
            MainApp.Instance.OnLevelWasLoadedEvent += OnLevelWasLoadedHandler;

            _appStateManager = GameClient.Get<IAppStateManager>();
            _uiManager = GameClient.Get<IUIManager>();

            OnLevelWasLoadedHandler(null);
        }

        public void Update()
        {
            switch(_appStateManager.AppState)
            { 
                case Enumerators.AppState.MAIN_MENU:
                    {
                        if (CurrentSceneName != "Menu" && !_isLoadingStarted)
                            ChangeScene(Enumerators.SceneType.MAIN_MENU);
                    }
                    break;
                default: break;
            }
        }

        public void ChangeScene(Enumerators.SceneType sceneType)
        {
            IsLoadedScene = false;
            _isLoadingStarted = true;

            switch (sceneType)
            {
                case Enumerators.SceneType.MAIN_MENU:
                    {
                        if (!_isLoadingScenesAsync)
                        {
#if UNITY_5_3_OR_NEWER
                            SceneManager.LoadScene("Menu");
#else
                            Application.LoadLevel("Menu");
#endif
                            
                        }
                        else
                            MainApp.Instance.StartCoroutine(LoadLevelAsync("Menu"));
                    }
                    break;
                default:
                    _isLoadingStarted = false;
                    break;
            }
        }

        private void OnLevelWasLoadedHandler(object param)
        {
#if UNITY_5_3_OR_NEWER
            CurrentSceneName = SceneManager.GetActiveScene().name;
#else
            CurrentSceneName = Application.loadedLevelName;
#endif
            _isLoadingStarted = false;
            IsLoadedScene = true;
            SceneLoadingProgress = 0;

            if (CurrentSceneName == "GamePart")
                _uiManager.HideAllPages();
        }

        private IEnumerator LoadLevelAsync(string levelName)
        {
#if UNITY_5_3_OR_NEWER
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelName);
#else
            AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
#endif
            while (!asyncOperation.isDone)
            {
                SceneLoadingProgress = Mathf.RoundToInt(asyncOperation.progress * 100f);
                yield return null;
            }
        }
    }
}