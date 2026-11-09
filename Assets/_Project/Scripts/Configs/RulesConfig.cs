using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Rules", fileName = "Rules")]
public class RulesConfig : ScriptableObject
{
    [field: Header("Conveyer:")]
    [field: SerializeField] public int ConveyerMaxBalls { get; private set; } = 24;
    [field: SerializeField] public float ConveyerNormalizedSpeed { get; private set; }
    [field: SerializeField] public float ConveyerFastNormalizedSpeed { get; private set; }
    [field: SerializeField] public List<float> ConveyerInputNormalizedPositions { get; private set; }


    [field: Header("Receivers:")]
    [field: SerializeField] public float ReceiverDistanceY { get; private set; }



}
