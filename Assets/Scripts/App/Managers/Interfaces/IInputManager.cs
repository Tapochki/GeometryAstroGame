using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IInputManager
    {
        event Action<Vector2> OnLeftMouseButtonClickEvent;
        event Action<Vector2> OnLeftMouseButtonOnUIClickEvent;
        event Action<Vector2> OnSwipeEvent;
        public Vector2 ReturnCameraMovementVelocity();
        public Vector2 MouseWorldPosition { get; }
        event Action OnRocketShootClickHandler;
        event Action OnLaserShootClickHandler;
        event Action OnMaskClickHandler;
        event Action OnDashClickHandler;
        public Vector2 MouseUiPosition { get; }
        event Action<float, float> OnMoveEvent;
        public Joystick CurrentJoystick { get; set; }
        public Joystick RotationJoystick { get; set; }

        public void OnRocketClick();
        public void OnMaskClick();
        public void OnLaserClick();
        public void OnDashClick();
    }
}
