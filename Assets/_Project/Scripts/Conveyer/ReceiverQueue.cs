using System.Collections.Generic;
using UnityEngine;

public class ReceiverQueue
{
    private Queue<ReceiverView> _receiverQueue = new();

    public readonly BallTrigger BallTrigger;

    public bool HasReceivers => _receiverQueue.Count > 0;
    public ColorType TopColorType => _receiverQueue.Peek().ColorType;


    public ReceiverQueue(BallTrigger ballTrigger)
    {
        BallTrigger = ballTrigger;
    }


    public void Initialize(List<ColorType> colorTypes)
    {
        foreach (var oldReceiver in _receiverQueue)
            Pools.Receivers.Return(oldReceiver);

        _receiverQueue.Clear();

        for (int i = 0; i < colorTypes.Count; i++)
        {
            var colorType = colorTypes[i];

            var receiver = Pools.Receivers.Get();
            receiver.Initialize(colorType);

            Vector2 topPosition = BallTrigger.transform.position;

            receiver.transform.position =
                 topPosition + Vector2.down * i * Configs.Rules.ReceiverDistanceY;

            _receiverQueue.Enqueue(receiver);
        }
    }



}
