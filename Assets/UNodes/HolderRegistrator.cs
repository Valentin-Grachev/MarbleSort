using System;
using UnityEngine;

namespace UNodes.Project
{
    public class HolderRegistrator : BaseHolderRegistrator
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private RoadView _road;
        [SerializeField] private BallView _ballPrefab;

        public override void RegisterItems(Action onRegistered)
        {
            var input = Register(new InputController(_camera));
            var level = Register(new LevelController(_road, _ballPrefab));


            
            onRegistered?.Invoke();
        }
    }
}
