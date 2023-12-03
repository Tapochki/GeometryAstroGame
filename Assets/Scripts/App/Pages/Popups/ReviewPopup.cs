using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class ReviewPopup : IUIPopup
    {
        private GameObject _selfObject;

        private ILoadObjectsManager _loadObjectManager;
        private IUIManager _uiManager;

        private Animator _animator;

        private OnBehaviourHandler _onBehaviourHandler;

        public void Init()
        {
            _loadObjectManager = GameClient.Get<ILoadObjectsManager>();
            _uiManager = GameClient.Get<IUIManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/ReviewPopup"), _uiManager.Canvas.transform);

            _animator = _selfObject.transform.Find("Image_Panel").GetComponent<Animator>();

            _onBehaviourHandler = _selfObject.transform.Find("Image_Panel").GetComponent<OnBehaviourHandler>();

            _onBehaviourHandler.OnAnimationStringEvent += OnAnimationStringEventHandler;

            Hide();
        }

        private void OnAnimationStringEventHandler(string obj)
        {
            switch (obj)
            {
                case "End":
                    Hide();
                    break;

                default:
                    break;
            }
        }

        public void Show()
        {
            _animator.Play("Empty", -1, 0);
            _selfObject.SetActive(true);
            _animator.Play("Start", -1, 0);
        }

        public void Show(object data)
        {
            Show();
        }

        public void Hide()
        {
            _animator.Play("Empty", -1, 0);
            _selfObject.SetActive(false);
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            _onBehaviourHandler.OnAnimationStringEvent -= OnAnimationStringEventHandler;
        }

        public void SetMainPriority()
        {
        }
    }
}