using System.Collections.Generic;
using UnityEngine;

public class ReceiverQueue
{
    private Queue<ReceiverView> _receiverQueue = new();

    private readonly Vector2 headWorldPosition;

    public bool HasReceivers => _receiverQueue.Count > 0;
    public ColorType TopColorType => _receiverQueue.Peek().ColorType;


    public ReceiverQueue(Vector2 headWorldPosition)
    {
        this.headWorldPosition = headWorldPosition;
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

            receiver.transform.position = 
                headWorldPosition + Vector2.down * i * Configs.Rules.ReceiverDistanceY; ;

            _receiverQueue.Enqueue(receiver);
        }
    }



}
