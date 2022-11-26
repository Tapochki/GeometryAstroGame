using System;
using System.Collections.Generic;
using UnityEngine;


namespace TandC.RunIfYouWantToLive
{
    public class InputManager : IService, IInputManager
    {
        public event Action<Vector2> OnLeftMouseButtonClickEvent;
        public event Action<Vector2> OnLeftMouseButtonOnUIClickEvent;

        private IGameplayManager _gameplayManager;

        private Vector2 _cameraMovementVelocity = Vector2.zero;

        public Vector2 MouseWorldPosition { get; private set; }
        public Vector2 MouseUiPosition { get; private set; }
        public Joystick CurrentJoystick { get; set ; }
        public Joystick RotationJoystick { get; set; }
        public event Action<float, float> OnMoveEvent;
        public event Action OnRocketShootClickHandler;
        public event Action OnMaskClickHandler;
        public event Action OnDashClickHandler;
        public event Action OnLaserShootClickHandler;

        private IUIManager _uIManager;
        private bool _mousePressed;

        public Vector2 ReturnCameraMovementVelocity()
        {
            return _cameraMovementVelocity;
        }
        public void Init()
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _uIManager = GameClient.Get<IUIManager>();

        }

        public void Update()
        {
            if (!_gameplayManager.IsGameplayStarted) 
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnLeftMouseButtonOnUIClickEvent?.Invoke(GetUIMousePosition());
                }
                return;
            }
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            OnMoveEvent?.Invoke(horizontal, vertical);
            MouseWorldPosition = GetMousePosition();
            if (Input.GetMouseButtonDown(0))
            {
                _mousePressed = true;
                //if (EventSystem.current.IsPointerOverGameObject())
                //{

                //}
                //   else
                //    {
                
                OnLeftMouseButtonClickEvent?.Invoke(GetMousePosition());
                //  }
            }
            if (_mousePressed)
            {
                //   MouseUiPosition = GetMouseUIPosition();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _mousePressed = false;
            }
        }
        //private Vector2 GetMouseUIPosition()
        //{
        //    return _uIManager.CameraUI.ScreenToViewportPoint(Input.mousePosition);
        //}

        public void Dispose()
        {

        }

        private Vector2 GetUIMousePosition()
        {
            return GameClient.Get<IUIManager>().UICamera.ScreenToWorldPoint(Input.mousePosition);
        }

        private Vector2 GetMousePosition()
        {
            return _gameplayManager.GameplayCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnRocketClick()
        {
            OnRocketShootClickHandler?.Invoke();
        }
        public void OnMaskClick()
        {
            OnMaskClickHandler?.Invoke();
        }
        public void OnLaserClick()
        {
            OnLaserShootClickHandler?.Invoke();
        }
        public void OnDashClick()
        {
            OnDashClickHandler?.Invoke();
        }
    }
}