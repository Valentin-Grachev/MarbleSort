using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Rules", fileName = "Rules")]
public class RulesConfig : ScriptableObject
{
    [field: Header("Emitters:")]
    [field: SerializeField] public float EmitterOneBallEmitDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float EmitterBallEmitDelay { get; private set; } = 0.1f;
    [field: SerializeField] public float EmitterBallStartVelocityY { get; private set; } = -2f;


    [field: Header("Conveyer:")]
    [field: SerializeField] public int ConveyerMaxBalls { get; private set; } = 24;
    [field: SerializeField] public float ConveyerNormalizedSpeed { get; private set; } = 0.22f;
    [field: SerializeField] public float ConveyerFastNormalizedSpeed { get; private set; } = 0.4f;
    [field: SerializeField] public Vector2 ConveyerInputNormalizedRange { get; private set; }
    [field: SerializeField] public float BallMoveToConveyerDuration { get; private set; } = 0.25f;


    [field: Header("Receivers:")]
    [field: SerializeField] public float ReceiverDistanceY { get; private set; } = 0.81f;
    [field: SerializeField] public float ReceiverMoveDuration { get; private set; } = 0.2f;
    [field: SerializeField] public float ReceiverFinishDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float BallMoveToReceiverDuration { get; private set; } = 0.4f;



}
