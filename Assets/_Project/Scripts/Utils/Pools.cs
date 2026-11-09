using UnityEngine;
using UNodes;

public class Pools
{
    public static Pool<Transform> ConveyerCells { get; private set; }

    public static Pool<BallView> Balls { get; private set; }
    public static Pool<EmitterView> Emitters { get; private set; }
    public static Pool<ReceiverView> Receivers { get; private set; }


    public Pools(BallView ballPrefab, EmitterView emitterPrefab, ReceiverView receiverPrefab) 
    {
        var poolContainer = new GameObject("[Pool]").transform;

        var conveyerCellPrefab = new GameObject("ConveyerCell").transform;
        ConveyerCells = new(conveyerCellPrefab, poolContainer);

        Balls = new(ballPrefab, poolContainer);
        Emitters = new(emitterPrefab, poolContainer);
        Receivers = new(receiverPrefab, poolContainer);
    }

    
}
