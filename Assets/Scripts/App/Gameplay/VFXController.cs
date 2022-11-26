using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;


namespace TandC.RunIfYouWantToLive
{
    public class VFXController : IController
    {
        private IGameplayManager _gameplayManager;
        private ILoadObjectsManager _loadObjectsManager;
        private IInputManager _inputManager;
        private IUIManager _UIManager;


        private List<VFXBase> _skillVFXes;

        private Transform _parentOfAllVFX;

        public VFXController()
        {
            _skillVFXes = new List<VFXBase>();
        }

        public void Dispose()
        {
           
        }

        public void Init()
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _inputManager = GameClient.Get<IInputManager>();
            _UIManager = GameClient.Get<IUIManager>();
            //     _gameplayController.GameConfigUpdatedEvent += GameConfigUpdatedEventHandler;
            _gameplayManager.GameplayStartedEvent += GameplayStartedEventHandler;
        }

        private void GameplayStartedEventHandler()
        {
            _parentOfAllVFX = _gameplayManager.GameplayObject.transform.Find("[VFX]");
        }

        public VFXBase PlaySkillVFX(Enumerators.SkillType skillType)
        {
            VFXBase vfxBase = null;
            switch(skillType)
            {
                case Enumerators.SkillType.Shield:
                    vfxBase = new ShieldSkillVFX(skillType);
                    break;
            }
            _skillVFXes.Add(vfxBase);

            return vfxBase;
        }

        public void ResetAll()
        {
            _skillVFXes.Clear();
        }

        public void SpawnHitParticles(Vector2 worldPosition, Vector3 angle)
        {
            GameObject hitObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/Particle_Hit"), _parentOfAllVFX);
            hitObject.transform.position = worldPosition;
            hitObject.transform.eulerAngles = new Vector3(0, 0, angle.z + 180f);
        }

        public void SpawnDeathParticles(Vector2 worldPosition)
        {
            GameObject deathObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/Particle_Death"), _parentOfAllVFX);
            deathObject.transform.position = worldPosition;
        }

        public void SpawnHitPlayerParticles(Vector2 enemyPosition, Vector2 worldPosition)
        {
            Vector2 direction = enemyPosition - worldPosition;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;


            GameObject vfx = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/HitPlayerEffect"), _parentOfAllVFX);
            vfx.transform.position = worldPosition;
            vfx.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public void SpawnImpulseHitPlayerParticles(Vector2 worldPosition)
        {
            GameObject vfx = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/ImpulseAttackVFX"), _parentOfAllVFX);
            vfx.transform.position = worldPosition;
        }

        public void SpawnFrozeBombVFX(Vector2 worldPosition)
        {
            GameObject vfx = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/FreezAllEnemiesVFX"), _parentOfAllVFX);
            vfx.transform.position = worldPosition;
        }

        public void SpawnFrozeEnemyVFX(Vector2 worldPosition, Transform parent)
        {
            GameObject vfx = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/FreezEnemyVFX"), parent);
            vfx.transform.position = worldPosition;
        }

        public GameObject SpawnRocketBlow(Vector2 worldPosition, float size)
        {
            GameObject RocketBlowObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/RocketBoomVFX"), _parentOfAllVFX);
            RocketBlowObject.transform.localScale = new Vector2(size, size);
            RocketBlowObject.transform.position = worldPosition;
            return RocketBlowObject;
        }
        public GameObject SpawnBombBlow(Vector2 worldPosition)
        {
            GameObject BombBlowObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/BombBoomVFX"), _parentOfAllVFX);
            BombBlowObject.transform.localScale = new Vector2(5,5);
            BombBlowObject.transform.position = worldPosition;
            return BombBlowObject;
        }

        public void SpawnDamagePointVFX(Vector2 worldPosition, float damageValue, Color textColor)
        {
            GameObject vfx = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/Gameplay/VFX/DamagePointVFX"), _parentOfAllVFX);
            var textItem = vfx.GetComponent<TextMesh>();
            textItem.text = ((int)damageValue).ToString();
            textItem.color = textColor;
            vfx.transform.position = worldPosition;
            vfx.GetComponent<OnBehaviourHandler>().OnAnimationStringEvent += (string value) =>
            {
                MonoBehaviour.Destroy(vfx);
            };
        }

        public VFXBase GetSkillVFXByType(Enumerators.SkillType type)
        {
            foreach (var item in _skillVFXes)
            {
                if (item.SkillType == type)
                    return item;
            }
            return null;
        }

        public void Update()
        {
            if (!_gameplayManager.IsGameplayStarted)
                return;

            for (int i = 0; i < _skillVFXes.Count; i++)
            {
                _skillVFXes[i].Update();
            }

            //if (Input.GetMouseButtonDown(0))
            //{
            //    PlayClickVFX(_inputManager.MouseWorldPosition);
            //}
        }

        public void FixedUpdate()
        {
           
        }
    }
}