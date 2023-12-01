using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class BossPopup : IUIPopup
    {
        private GameObject _selfObject;

        private ILoadObjectsManager _loadObjectManager;
        private IUIManager _uiManager;

        private GameObject _bossAppearContainer;
        private GameObject _bossActionContainer;
        private GameObject _bossDefeatContainer;

        private Image _bossHealthBarImage;

        private float _bossMaxHealth;

        // TODO - add canvas group reference

        public void Init()
        {
            _loadObjectManager = GameClient.Get<ILoadObjectsManager>();
            _uiManager = GameClient.Get<IUIManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/BossPopup"), _uiManager.Canvas.transform);

            _bossAppearContainer = _selfObject.transform.Find("Container_BossAppear").gameObject;
            _bossActionContainer = _selfObject.transform.Find("Container_BossAction").gameObject;
            _bossDefeatContainer = _selfObject.transform.Find("Container_BossDefeat").gameObject;

            _bossHealthBarImage = _bossActionContainer.transform.Find("Image_HealthBarBackground/Image_HealthBar/Image_Fill").GetComponent<Image>();
        }

        public void Show()
        {
            _selfObject.SetActive(true);

            ChangeBossHealthBarState(BossHealthBarState.None);
        }

        /// <summary>
        /// Need in data transfer float value of Boss Max Health
        /// </summary>
        /// <param name="data"></param>
        public void Show(object data)
        {
            _selfObject.SetActive(true);

            _bossMaxHealth = (float)data;
        }

        public void Hide()
        {
            _selfObject.SetActive(false);

            ChangeBossHealthBarState(BossHealthBarState.None);
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            ChangeBossHealthBarState(BossHealthBarState.None);
        }

        public void SetMainPriority()
        {
        }

        public void UpdateBossHealthBar(float bossCurrentHealth)
        {
            _bossHealthBarImage.fillAmount = (bossCurrentHealth / _bossMaxHealth) * 100;

            if (bossCurrentHealth <= 0)
            {
                ChangeBossHealthBarState(BossHealthBarState.Defeat);
            }
        }

        //
        // TODO - for animation use DotTween
        //

        // TODO - add animation of panel when boss appear
        public void BossAppear()
        {
            ChangeBossHealthBarState(BossHealthBarState.Appear);
        }

        // TODO - add animation to health bar fill image when boss take damage
        public void BossAction()
        {
            ChangeBossHealthBarState(BossHealthBarState.Action);
        }

        // TODO - add animation of panel when boss defeat
        public void BossDefeat()
        {
            ChangeBossHealthBarState(BossHealthBarState.Defeat);
        }

        private void ChangeBossHealthBarState(BossHealthBarState state)
        {
            _bossAppearContainer.SetActive(state == BossHealthBarState.Appear);
            _bossActionContainer.SetActive(state == BossHealthBarState.Action);
            _bossDefeatContainer.SetActive(state == BossHealthBarState.Defeat);
        }

        public enum BossHealthBarState
        {
            Appear,
            Action,
            Defeat,

            None,
        }
    }
}