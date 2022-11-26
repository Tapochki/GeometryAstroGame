using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    public class VFXBase
    {
        protected IGameplayManager _gameplayManager;
        protected ObjectsController _objectsController;
        protected VFXController _vfxController;
        protected PlayerController _playerController;

        public Enumerators.SkillType SkillType { get; private set; }

        public VFXBase(Enumerators.SkillType type)
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _objectsController = _gameplayManager.GetController<ObjectsController>();
            _vfxController = _gameplayManager.GetController<VFXController>();
            _playerController = _gameplayManager.GetController<PlayerController>();

            SkillType = type;
        }

        public virtual void Update()
        {

        }
    }
}