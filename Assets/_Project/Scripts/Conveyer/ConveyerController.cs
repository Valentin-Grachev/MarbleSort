using System.Collections.Generic;
using UnityEngine;
using UNodes;

public class ConveyerController : IUpdateHandler
{
    private class ConveyerCell
    {
        public BallView ball;
        public Transform transform;
    }

    private List<ConveyerCell> _conveyerCells = new();
    private List<BallView> _inputBalls = new();

    private const int RECEIVERS_AMOUNT = 4;

    private readonly ConveyerView conveyer;
    private readonly CycleMovingProcessor<ConveyerCell> cellMovingProcessor;
    private readonly List<ReceiverQueue> receiverQueues;


    public ConveyerController(ConveyerView conveyer, 
        BallTrigger conveyerBallTrigger, List<BallTrigger> receiverTriggers)
    {
        this.conveyer = conveyer;

        float ballNormalizedDistance = 1f / Configs.Rules.ConveyerMaxBalls;

        cellMovingProcessor = new(
            Configs.Rules.ConveyerNormalizedSpeed,
            Configs.Rules.ConveyerFastNormalizedSpeed, 
            Configs.Rules.ConveyerMaxBalls);


        conveyerBallTrigger.OnBallEnter += OnConveyerBallEnter;
        conveyerBallTrigger.OnBallExit += OnConveyerBallExit;

        receiverQueues = new(RECEIVERS_AMOUNT);

        for (int i = 0; i < RECEIVERS_AMOUNT; i++)
        {
            var trigger = receiverTriggers[i];
            var receiverQueue = new ReceiverQueue(trigger.transform.position);

            trigger.OnBallEnter += OnReceiverBallEnter;
            receiverQueues.Add(receiverQueue);

            void OnReceiverBallEnter(BallView ball)
                => OnReceiverQueueBallEnter(receiverQueue, ball);
        }
    }

    void IUpdateHandler.Update()
    {
        UpdateMovingConveyerCells();
        UpdateAddingBallToConveyer();
    }


    private void OnReceiverQueueBallEnter(ReceiverQueue receiverQueue, BallView ball)
    {

    }

    private void OnConveyerBallEnter(BallView ball)
    {
        if (!_inputBalls.Contains(ball))
            _inputBalls.Add(ball);
    }

    private void OnConveyerBallExit(BallView ball)
    {
        _inputBalls.Remove(ball);
    }

    
    private void UpdateMovingConveyerCells()
    {
        cellMovingProcessor.Update(Time.deltaTime);

        foreach (var cellPoint in cellMovingProcessor.PointPositions)
        {
            cellPoint.Key.transform.position =
                conveyer.MovingSpline.EvaluatePosition(cellPoint.Value);
        }
    }

    private void UpdateAddingBallToConveyer()
    {
        if (_inputBalls.Count == 0) return;

        var ball = _inputBalls[0];

        foreach (var inputNormalizedPosition in Configs.Rules.ConveyerInputNormalizedPositions)
        {
            if (cellMovingProcessor.AddPointIsAvailable(inputNormalizedPosition))
            {
                AddBallToConveyer(ball, inputNormalizedPosition);
                break;
            }
        }
    }

    private void AddBallToConveyer(BallView ball, float normalizedPosition)
    {
        var cell = new ConveyerCell()
        {
            ball = ball,
            transform = Pools.ConveyerCells.Get(),
        };

        cell.transform.position = 
            conveyer.MovingSpline.EvaluatePosition(normalizedPosition);

        cellMovingProcessor.AddPoint(cell, normalizedPosition);
        ball.AnimateMoveToConveyerCell(cell.transform);
        ball.SetPhysics(false);

        _inputBalls.Remove(ball);
    }



    


}
