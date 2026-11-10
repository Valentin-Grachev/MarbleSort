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


    private readonly int maxPoints;
    private readonly float pointMoveDistance;
    private readonly float pointInsertRadius;
    private readonly float slowMoveSpeed;
    private readonly float fastMoveSpeed;
    private readonly Vector2 insertRange;

    public IReadOnlyList<PointState> PointStates => _orderedPoints;


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
        if (_orderedPoints.Count == 0)
        {
            position = (from + to) / 2f;
            InsertPoint(component, position);
        }
        else
        {
            bool success = TryFindPositionForInsertPointInRange(from, to, out position);
            if (success) InsertPoint(component, position);
            else throw new Exception("No space enough for insert!");
        }
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

    }



    public void Update(float deltaTime)
    {
        if (_orderedPoints.Count == 0) return;

        float slowMovement = slowMoveSpeed * deltaTime;
        float fastMovement = fastMoveSpeed * deltaTime;

        var headPointState = _orderedPoints[0];

        headPointState.position = (headPointState.position + slowMovement) % 1f;

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            var pointState = _orderedPoints[i];

            float currentDistanceToHead = GetForwardDistance
                (pointState.position, headPointState.position);

            float targetDistanceToHead = pointMoveDistance * i;

            bool useFastSpeed = currentDistanceToHead > targetDistanceToHead;
            float movement = useFastSpeed ? fastMovement : slowMovement;

            pointState.position = (pointState.position + movement) % 1f;
        }
    }


    public void Clear()
    {

    }


    private bool TryFindPositionForInsertPointInRange
        (float minBorder, float maxBorder, out float result)
    {
        result = 0f;

        float minFilterPosition = minBorder - pointInsertRadius;
        float maxFilterPosition = maxBorder + pointInsertRadius;
        float pointDiameter = pointInsertRadius * 2f;

        float candidatePosition = minBorder + pointInsertRadius;

        while (candidatePosition + pointInsertRadius <= maxBorder)
        {
            bool positionIsDefined = true;

            foreach (var pointState in _orderedPoints)
            {
                float pointPosition = pointState.position;

                if (pointPosition < minFilterPosition || maxFilterPosition < pointPosition)
                    continue;

                float distance = Mathf.Abs(candidatePosition - pointPosition);

                if (distance < pointDiameter)
                {
                    candidatePosition = pointPosition + pointDiameter;
                    positionIsDefined = false;
                    break;
                }
            }

            if (positionIsDefined)
            {
                result = candidatePosition;
                return true;
            }
        }

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



}
