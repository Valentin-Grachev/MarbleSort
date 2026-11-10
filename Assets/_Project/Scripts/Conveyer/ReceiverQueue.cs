using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReceiverQueue
{
    private Queue<ReceiverView> _receiverQueue = new();

    public readonly BallTrigger BallTrigger;

    public bool HasReceivers => _receiverQueue.Count > 0;
    public ReceiverView TopReceiver => _receiverQueue.Peek();


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
            receiver.transform.SetParent(BallTrigger.transform);

            receiver.transform.localPosition = Vector2.down * i * Configs.Rules.ReceiverDistanceY;

            _receiverQueue.Enqueue(receiver);
        }
    }

    public void MoveNext()
    {
        _receiverQueue.Dequeue();

        foreach (var receiver in _receiverQueue)
        {
            var toPositionY = receiver.transform.localPosition.y + Configs.Rules.ReceiverDistanceY;

            receiver.transform.DOLocalMoveY(toPositionY, Configs.Rules.ReceiverMoveDuration)
                .SetEase(Ease.OutQuad);
        }
    }




}
