using System;
using UnityEngine;

namespace UNodes.Project
{
    public class HolderRegistrator : BaseHolderRegistrator
    {
        [SerializeField] private RoadView _road;

        public override void RegisterItems(Action onRegistered)
        {
            var level = Register(new LevelController(_road));

            
            onRegistered?.Invoke();
        }
    }
}
