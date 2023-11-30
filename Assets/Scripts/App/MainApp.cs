using System;
using TandC.RunIfYouWantToLive.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TandC.RunIfYouWantToLive
{
    public class MainApp : MonoBehaviour
    {
        public delegate void MainAppDelegate(object param);
        public event MainAppDelegate OnLevelWasLoadedEvent;

        public event Action LateUpdateEvent;
        public event Action FixedUpdateEvent;

        private FPSCounter _fPSCounter;
        [SerializeField] private TextMeshProUGUI _fpsText;

        private static MainApp _Instance;
        public static MainApp Instance
        {
            get { return _Instance; }
            private set { _Instance = value; }
        }

        float deltaTime = 0.0f;

        [SerializeField] private bool _isShowFps;
        [SerializeField] private int _fpsLimit = 60;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _fPSCounter = new FPSCounter(_fpsText);

            Instance = this;
            Application.targetFrameRate = _fpsLimit;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Instance == this)
            {
                GameClient.Instance.InitServices();
                GameClient.Get<IAppStateManager>().ChangeAppState(Common.Enumerators.AppState.APP_INIT_LOADING);
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            }
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            if (Instance == this)
            {
                GameClient.Instance.Update();

                if (_isShowFps)
                {
                    _fPSCounter.Update();
                }
            }
        }

        private void LateUpdate()
        {
            if (Instance == this)
            {
                if (LateUpdateEvent != null)
                {
                    LateUpdateEvent();
                }
            }
        }

        private void FixedUpdate()
        {
            if (Instance == this)
            {
                if (FixedUpdateEvent != null)
                {
                    FixedUpdateEvent();
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                GameClient.Instance.Dispose();
            }
        }
        private void OnApplicationQuit()
        {
            if (Instance == this)
            {
                GameClient.Instance.Dispose();
            }
        }


        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (Instance == this)
            {
                if (OnLevelWasLoadedEvent != null)
                {
                    OnLevelWasLoadedEvent(arg0.buildIndex);
                }
            }
        }
    }
}