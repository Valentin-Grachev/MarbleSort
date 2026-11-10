using System;
using System.Collections.Generic;
using UnityEngine;

namespace UNodes.Project
{
    public class HolderRegistrator : BaseHolderRegistrator
    {
        [SerializeField] private EnableValue<int> _targetFrameRate;
        [Header("Scene:")]
        [SerializeField] private Camera _camera;
        [SerializeField] private ConveyerView _conveyer;
        [SerializeField] private Grid _emitterGrid;
        [SerializeField] private List<BallTrigger> _receiverTriggers;
        [Header("Prefabs:")]
        [SerializeField] private BallView _ballPrefab;
        [SerializeField] private EmitterView _emitterPrefab;
        [SerializeField] private ReceiverView _receiverPrefab;


        public override void RegisterItems(Action onRegistered)
        {
            Application.targetFrameRate = _targetFrameRate.enabled ?
                _targetFrameRate.value : 1000;

            new Pools(_ballPrefab, _emitterPrefab, _receiverPrefab);

            var level = Register(new LevelController());
            var input = Register(new InputController(_camera));
            var conveyer = Register(new ConveyerController(level, _emitterGrid, _conveyer, _receiverTriggers));
            var start = Register(new StartController(level));



            onRegistered?.Invoke();
        }
    }
}
