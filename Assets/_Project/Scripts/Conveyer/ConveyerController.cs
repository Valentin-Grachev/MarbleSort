using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UNodes;
using UNodes.Extensions;

public class ConveyerController : IUpdateHandler
{
    private class ConveyerCell
    {
        public BallView ball;
        public Transform transform;
    }

    private List<BallView> _inputBalls = new();
    private List<BallView> _conveyerBalls = new();

    private readonly CycleMovingProcessor<ConveyerCell> conveyerMovingProcessor;
    private readonly List<ReceiverQueue> receiverQueues;
    private readonly ConveyerView conveyer;
    private readonly LevelBuilder levelBuilder;
    private readonly LevelController levelController;


    public ConveyerController(LevelController levelController, Grid emitterGrid, LockPixelSprites lockPixelSprites,
        Tilemap lockPixelTilemap, ConveyerView conveyer, List<BallTrigger> receiverTriggers)
    {
        this.levelController = levelController;
        levelController.OnLevelStarted += OnLevelStarted;

        float ballNormalizedDistance = 1f / Configs.Rules.ConveyerMaxBalls;

        conveyerMovingProcessor = new(
            Configs.Rules.ConveyerNormalizedSpeed,
            Configs.Rules.ConveyerFastNormalizedSpeed, 
            Configs.Rules.ConveyerMaxBalls);

        var inputBallTrigger = conveyer.InputBallTrigger;
        inputBallTrigger.OnBallEnter += OnConveyerBallEnter;

        this.conveyer = conveyer;

        receiverQueues = new List<ReceiverQueue>();
        foreach (var receiverTrigger in receiverTriggers)
        {
            var receiverQueue = new ReceiverQueue(receiverTrigger);
            receiverQueue.BallTrigger.OnBallEnter += OnReceiverBallEnter;

            receiverQueues.Add(receiverQueue);

            void OnReceiverBallEnter(BallView ball)
                => OnReceiverQueueBallEnter(receiverQueue, ball);
        }

        levelBuilder = new LevelBuilder
            (emitterGrid, lockPixelTilemap, lockPixelSprites, receiverQueues);
    }

    private void OnLevelStarted()
    {
        Pools.Balls.Clear();
        Pools.ConveyerCells.Clear();
        Pools.Emitters.Clear();
        conveyerMovingProcessor.Clear();
        _inputBalls.Clear();
        _conveyerBalls.Clear();

        var levelData = Configs.Levels.GetLevelData(levelController.Level);
        levelBuilder.BuildLevel(levelData);
    }

    void IUpdateHandler.Update()
    {
        UpdateMovingConveyerCells();
        UpdateAddingBallToConveyer();
    }

    private void OnReceiverQueueBallEnter(ReceiverQueue receiverQueue, BallView ball)
    {
        if (!receiverQueue.HasReceivers) return;

        var receiver = receiverQueue.TopReceiver;
        if (receiver.ColorType != ball.ColorType || receiverQueue.TopReceiver.IsFull)
            return;

        var cellTransform = receiver.GetCurrentCellTransform();
        int displayedBalls = receiver.BallsAmount + 1;

        _conveyerBalls.Remove(ball);

        ball.MoveToReceiverCell(cellTransform, onCompleted: () =>
        {
            receiver.DisplayBallFullness(displayedBalls);
            Pools.Balls.Return(ball);

            if (receiver.IsDisplayFull) 
                OnReceiverCompleted(receiverQueue, receiver);
        });

        receiver.IncreaseBallCount();

        var conveyerCell = conveyerMovingProcessor.PointStates
            .FindOrExeption(state => state.component.ball == ball).component;

        conveyerMovingProcessor.RemovePoint(conveyerCell);
        Pools.ConveyerCells.Return(conveyerCell.transform);
    }

    private void OnConveyerBallEnter(BallView ball)
    {
        if (!_inputBalls.Contains(ball) && !_conveyerBalls.Contains(ball))
            _inputBalls.Add(ball);
    }


    private void OnReceiverCompleted(ReceiverQueue receiverQueue, ReceiverView receiver)
    {
        receiver.AnimateFinish();
        receiverQueue.MoveNext();
    }


    
    private void UpdateMovingConveyerCells()
    {
        conveyerMovingProcessor.Update(Time.deltaTime);

        foreach (var pointState in conveyerMovingProcessor.PointStates)
        {
            Vector3 worldPosition = conveyer.MovingSpline.EvaluatePosition(pointState.position);
            pointState.component.transform.position = worldPosition;
        }
    }

    private void UpdateAddingBallToConveyer()
    {
        if (_inputBalls.Count == 0) return;

        var ball = _inputBalls[0];
        Vector2 inputRange = Configs.Rules.ConveyerInputNormalizedRange;

        if (conveyerMovingProcessor.InsertToRangeIsAvailable(inputRange.x, inputRange.y))
        {
            var cell = new ConveyerCell()
            {
                ball = ball,
                transform = Pools.ConveyerCells.Get(),
            };

            conveyerMovingProcessor.InsertPointToRange
                (cell, inputRange.x, inputRange.y, out float position);

            cell.transform.position = conveyer.MovingSpline.EvaluatePosition(position);

            ball.MoveToConveyerCell(cell.transform);

            _inputBalls.Remove(ball);
            _conveyerBalls.Add(ball);
        }
    }

    
}
