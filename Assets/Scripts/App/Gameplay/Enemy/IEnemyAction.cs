using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{

    public interface IEnemyAction : IEnemyPart
    {
        void Action();
    }

    public interface IEnemyMove : IEnemyPart
    {
        bool CanAction { get; }
        void Move();
    }

    public interface IEnemyPart 
    {
        void Init(Enemy enemy);
        void Update();
    }

}


