using System;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IGameplayManager
    {
        event Action GameplayStartedEvent;
        event Action GameplayEndedEvent;
        event Action ControllerInitEvent;
        bool IsGameplayStarted { get; }

        int PlayerMoney { get; }

        GameObject GameplayObject { get; }
        GameplayData GameplayData { get; }
        ShopData ShopData { get; }

        bool IsGamePaused { get; }

        Camera GameplayCamera { get; }

        T GetController<T>() where T : IController;
        GameObject GetSelectedProduct(Enumerators.CustomisationType type);
        void StartGameplay();
        void StopGameplay();
        void RestartGameplay();
        void PauseGame(bool enablePause);
        void PauseOff();

        void PlayerGetMoney(int value);
    }
}