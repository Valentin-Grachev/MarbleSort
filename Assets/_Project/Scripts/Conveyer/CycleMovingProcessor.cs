using System;
using System.Collections.Generic;
using UnityEngine;

public class CycleMovingProcessor<TPointComponent> where TPointComponent : class
{
    public class PointState
    {
        public TPointComponent component;
        public float position;
    }

    private List<PointState> _orderedPoints = new();
    private List<float> _occupiedInputPositions = new();


    private readonly int maxPoints;
    private readonly float pointMoveDistance;
    private readonly float pointInsertRadius;
    private readonly float slowMoveSpeed;
    private readonly float fastMoveSpeed;


    public List<PointState> PointStates => _orderedPoints;


    public CycleMovingProcessor(float slowMoveSpeed, float fastMoveSpeed, int maxPoints)
    {

        this.maxPoints = maxPoints;
        this.slowMoveSpeed = slowMoveSpeed;
        this.fastMoveSpeed = fastMoveSpeed;

        pointMoveDistance = 1f / maxPoints;
        pointInsertRadius = pointMoveDistance / 2f * 0.99f;
    }

    public bool InsertToRangeIsAvailable(float from, float to)
        => TryFindPositionForInsertPointInRange(from, to, out _);



    public void InsertPointToRange(TPointComponent component, float from, float to, out float position)
    {
        bool success = TryFindPositionForInsertPointInRange(from, to, out position);

        if (success) InsertPoint(component, position);
        else throw new Exception("No space enough for insert!");
    }

    public void InsertPoint(TPointComponent component, float position)
    {
        int orderIndex = GetOrderIndexForInsertPoint(position);
        _orderedPoints.Insert(orderIndex, new PointState()
        {
            component = component,
            position = position
        });
    }



    public void RemovePoint(TPointComponent component)
    {
        var pointState = _orderedPoints.Find(pointState => pointState.component == component);
        _orderedPoints.Remove(pointState);
    }



    public void Update(float deltaTime)
    {
        if (_orderedPoints.Count == 0) return;

        float maxMovement = pointMoveDistance * 0.45f;

        float slowMovement = Mathf.Min(slowMoveSpeed * deltaTime, maxMovement);
        float fastMovement = Mathf.Min(fastMoveSpeed * deltaTime, maxMovement);

        var headPointState = _orderedPoints[0];
        headPointState.position = CyclePosition(headPointState.position + slowMovement);

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            var pointState = _orderedPoints[i];

            float targetDistanceToHead = pointMoveDistance * i;

            float newPosition = CyclePosition(pointState.position + fastMovement);
            float newDistanceToHead = GetForwardDistance(newPosition, headPointState.position);

            if (newDistanceToHead < targetDistanceToHead)
                pointState.position = CyclePosition(headPointState.position - targetDistanceToHead);

            else pointState.position = newPosition;
        }
    }


    public void Clear() => _orderedPoints.Clear();



    private bool TryFindPositionForInsertPointInRange
        (float minBorder, float maxBorder, out float insertPosition)
    {
        insertPosition = 0f;

        _occupiedInputPositions?.Clear();
        _occupiedInputPositions.Add(minBorder);
        _occupiedInputPositions.Add(maxBorder);

        float minFilterPosition = minBorder - pointInsertRadius;
        float maxFilterPosition = maxBorder + pointInsertRadius;

        foreach (var pointState in _orderedPoints)
        {
            float pointPosition = pointState.position;

            if (minFilterPosition <= pointPosition && pointPosition <= maxFilterPosition)
                _occupiedInputPositions.Add(pointState.position);
        }

        _occupiedInputPositions.Sort();

        float maxDistance = 0f;
        for (int i = 0; i < _occupiedInputPositions.Count - 1; i++)
        {
            float from = _occupiedInputPositions[i];
            float to = _occupiedInputPositions[i + 1];
            float distance = to - from;

            if (maxDistance < distance)
            {
                maxDistance = distance;
                insertPosition = (from + to) / 2f;
            }
        }

        if (maxDistance > pointInsertRadius * 4f)
            return true;

        return false;
    }


    private int GetOrderIndexForInsertPoint(float position)
    {
        if (_orderedPoints.Count == 0) return 0;

        float headPosition = _orderedPoints[0].position;
        float distanceToHead = GetForwardDistance(position, headPosition); 

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            float pointPosition = _orderedPoints[i].position;
            float otherDistanceToHead = GetForwardDistance(pointPosition, headPosition);

            if (distanceToHead < otherDistanceToHead) 
                return i;
        }

        return _orderedPoints.Count;
    }

    private float GetForwardDistance(float from, float to)
        => from < to ? to - from : 1f - from + to;

    private float CyclePosition(float position)
    {
        float result = position % 1f;
        if (result < 0f) result += 1f;

        return result;
    }


}
