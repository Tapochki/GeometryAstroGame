using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class GameplayManager : IService, IGameplayManager
    {
        public event Action GameplayStartedEvent;

        public event Action ControllerInitEvent;

        public event Action GameplayEndedEvent;

        public int PlayerMoney { get; private set; }

        private List<IController> _controllers;

        private ILoadObjectsManager _loadObjectsManager;
        private ITimerManager _timerManager;
        private IDataManager _dataManager;

        public GameObject GameplayObject { get; private set; }

        public Camera GameplayCamera { get; private set; }

        public GameplayData GameplayData { get; private set; }
        public ShopData ShopData { get; private set; }
        public bool IsGameplayStarted { get; private set; }
        public bool IsGamePaused { get; private set; }

        private bool _isAfterPause;
        private float _afterPauseTimer;
        private const float _pauseTimeRecover = 0.5f;

        public void Dispose()
        {
            StopGameplay();

            if (_controllers != null)
            {
                foreach (var item in _controllers)
                    item.Dispose();
            }

            // _loadObjectsManager.BundlesDataLoadedEvent -= BundlesDataLoadedEventHandler;
            // _loadObjectsManager.BundlesDataLoadFailedEvent -= BundlesDataLoadFailedEventHandler;
        }

        private void OnDataManagerEndLoadCache() 
        {
            PlayerMoney = _dataManager.CachedUserLocalData.MoneyCount;
            foreach (var item in ShopData.UpgradeDataList)
            {
                item.CurrentLevel = _dataManager.CachedUserLocalData.PlayerCharacteristicsData[item.UpgradeType];
            }
        }

        public void Init()
        {
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _timerManager = GameClient.Get<ITimerManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _dataManager.EndLoadCache += OnDataManagerEndLoadCache;
            _controllers = new List<IController>()
            {
                 new PlayerController(),
                 new ObjectsController(),
                 new VFXController(),
                 new EnemyController(),
                 new SkillsController()
            };
            ShopData = _loadObjectsManager.GetObjectByPath<ShopData>("Data/ShopData");
//#if UNITY_EDITOR
//            foreach (var item in ShopData.UpgradeDataList) 
//            {
//                item.CurrentLevel = 0;
//            }
//#endif
            foreach (var item in _controllers)
                item.Init();
        }

        public void Update()
        {
            if (!IsGamePaused)
            {
                if (_controllers != null)
                {
                    foreach (var item in _controllers)
                        item.Update();
                }
            }
            else 
            {
                if (_isAfterPause) 
                {
                    _afterPauseTimer -= Time.unscaledDeltaTime;
                    if (_afterPauseTimer <= 0)
                    {
                        PauseOff();
                    }
                }

            }
        }
        public void FixedUpdate()
        {
            if (!IsGamePaused)
            {
                if (_controllers != null)
                {
                    foreach (var item in _controllers)
                        item.FixedUpdate();
                }
            }
        }

        public T GetController<T>() where T : IController
        {
            foreach (var item in _controllers)
            {
                if (item is T)
                {
                    return (T)item;
                }
            }

            throw new Exception("Controller " + typeof(T).ToString() + " have not implemented");
        }

        public GameObject GetSelectedProduct(Enumerators.CustomisationType type) 
        {
            return ShopData.GetProductsByType(type).GetProductById(_dataManager.CachedUserLocalData.GetSelectedProduct(type)).Prefab;
        }

        public void PauseGame(bool enablePause)
        {
            if (enablePause)
            {
                Time.timeScale = 0;
                IsGamePaused = true;
                _isAfterPause = false;
            }
            else
            {
                _afterPauseTimer = _pauseTimeRecover;
                _isAfterPause = true;
            }
        }

        public void PauseOff() 
        {
            _isAfterPause = false;
            Time.timeScale = 1;
            IsGamePaused = false;
        }

        public void UpdateBulletImage() 
        {
            foreach(var bullet in GameplayData.bulletData) 
            {
                if (bullet.IsNotCustomise) 
                {
                    continue;
                }
                bullet.ButlletObject = GetSelectedProduct(bullet.CustomisationType);
            }
        }

        public void StartGameplay()
        {
            if (IsGameplayStarted)
                return;
            GameplayObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/Gameplay"));
            GameplayCamera = GameplayObject.transform.Find("GameplayCamera").GetComponent<Camera>();
            GameplayData = _loadObjectsManager.GetObjectByPath<GameplayData>("Data/GameplayData");
            UpdateBulletImage();
            MainApp.Instance.FixedUpdateEvent += FixedUpdate;
            // GetController<GameplayController>().StartGameplay();
            IsGameplayStarted = true;
            _isAfterPause = false;
            ControllerInitEvent?.Invoke();
            GameplayStartedEvent?.Invoke();
        }

        public void StopGameplay()
        {
            if (!IsGameplayStarted)
                return;

            // GetController<GameplayController>().StopGameplay();

            foreach (var item in _controllers)
                item.ResetAll();

            IsGameplayStarted = false;
            MainApp.Instance.FixedUpdateEvent -= FixedUpdate;
            MonoBehaviour.Destroy(GameplayObject);
        }

        public void RestartGameplay()
        {
            StopGameplay();
            StartGameplay();
        }

        public void PlayerGetMoney(int value) 
        {
            PlayerMoney += value;
            _dataManager.CachedUserLocalData.MoneyCount = PlayerMoney;
            _dataManager.SaveCache(Common.Enumerators.CacheDataType.USER_LOCAL_DATA);
        }
    }
}