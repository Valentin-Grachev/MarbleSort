using System;
using System.Collections.Generic;
using UnityEngine;

namespace UNodes.Project
{
    public class HolderRegistrator : BaseHolderRegistrator
    {
        [Header("Scene:")]
        [SerializeField] private Camera _camera;
        [SerializeField] private ConveyerView _conveyer;
        [SerializeField] private Grid _emitterGrid;
        [SerializeField] private BallTrigger _conveyerTrigger;
        [SerializeField] private List<BallTrigger> _receiverTriggers;
        [Header("Prefabs:")]
        [SerializeField] private BallView _ballPrefab;
        [SerializeField] private EmitterView _emitterPrefab;
        [SerializeField] private ReceiverView _receiverPrefab;


        public override void RegisterItems(Action onRegistered)
        {
            new Pools(_ballPrefab, _emitterPrefab, _receiverPrefab);


            var input = Register(new InputController(_camera));
            var level = Register(new LevelController(_emitterGrid, _receiverTriggers));

            var conveyer = Register(new ConveyerController
                (_conveyer, _conveyerTrigger, _receiverTriggers));

            var start = Register(new StartController(level));



            onRegistered?.Invoke();
        }
    }
}
