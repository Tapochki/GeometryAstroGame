namespace TandC.RunIfYouWantToLive
{
    public class GameClient : ServiceLocatorBase
    {
        private static object _sync = new object();

        private static GameClient _Instance;
        public static GameClient Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_sync)
                    {
                        _Instance = new GameClient();
                    }
                }
                return _Instance;
            }
        }

        public static bool IsDebugMode = false; //change to 'false' for release

        /// <summary>
        /// Initializes a new instance of the <see cref="GameClient"/> class.
        /// </summary>
        internal GameClient()
            : base()
        {
#if UNITY_EDITOR
            IsDebugMode = true;
#endif
            AddService<ITimerManager>(new TimerManager());
            AddService<IAdvarismetnManager>(new AdvarismetnManager());
            AddService<ILocalizationManager>(new LocalizationManager());
            AddService<INetworkManager>(new NetworkManager());
            AddService<ILoadObjectsManager>(new LoadObjectsManager());
            AddService<IAppStateManager>(new AppStateManager());
            AddService<ISoundManager>(new SoundManager());
            AddService<IUIManager>(new UIManager());
            AddService<IScenesManager>(new ScenesManager());
            AddService<IDataManager>(new DataManager());
            AddService<IGameplayManager>(new GameplayManager());
            AddService<IInputManager>(new InputManager());
        }

        public static T Get<T>()
        {
            return Instance.GetService<T>();
        }
    }
}