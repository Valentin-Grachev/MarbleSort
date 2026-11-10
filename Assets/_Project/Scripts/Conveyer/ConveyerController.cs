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

    private readonly LevelController levelController;
    private readonly CycleMovingProcessor<ConveyerCell> cellMovingProcessor;


    public ConveyerController(LevelController levelController)
    {
        this.levelController = levelController;

        float ballNormalizedDistance = 1f / Configs.Rules.ConveyerMaxBalls;

        cellMovingProcessor = new(
            Configs.Rules.ConveyerNormalizedSpeed,
            Configs.Rules.ConveyerFastNormalizedSpeed, 
            Configs.Rules.ConveyerMaxBalls);

        var inputBallTrigger = levelController.Conveyer.InputBallTrigger;
        inputBallTrigger.OnBallEnter += OnConveyerBallEnter;
        inputBallTrigger.OnBallExit += OnConveyerBallExit;

        foreach (var receiverQueue in levelController.ReceiverQueues)
        {
            receiverQueue.BallTrigger.OnBallEnter += OnReceiverBallEnter;

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
        Debug.Log(receiverQueue.TopColorType);
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

        foreach (var pointState in cellMovingProcessor.PointStates)
        {
            Vector3 worldPosition = levelController.Conveyer
                .MovingSpline.EvaluatePosition((float)pointState.position);

            pointState.component.transform.position = worldPosition;
        }
    }

    private void UpdateAddingBallToConveyer()
    {
        if (_inputBalls.Count == 0) return;

        var ball = _inputBalls[0];
        Vector2 inputRange = Configs.Rules.ConveyerInputNormalizedRange;

        if (cellMovingProcessor.InsertToRangeIsAvailable(inputRange.x, inputRange.y))
        {
            var cell = new ConveyerCell()
            {
                ball = ball,
                transform = Pools.ConveyerCells.Get(),
            };

            cellMovingProcessor.InsertPointToRange
                (cell, inputRange.x, inputRange.y, out float position);

            cell.transform.position = levelController.Conveyer
                .MovingSpline.EvaluatePosition(position);

            ball.AnimateMoveToConveyerCell(cell.transform);
            ball.SetPhysics(false);

            _inputBalls.Remove(ball);

        }
    }



}
