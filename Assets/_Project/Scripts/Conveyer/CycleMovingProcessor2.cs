using System;
using System.Collections.Generic;

public class CycleMovingProcessor2<TPoint> where TPoint : class
{
    private Dictionary<TPoint, double> _pointPositions = new();
    private List<TPoint> _orderedPoints = new();


    private readonly int maxPoints;
    private readonly double pointDistance;
    private readonly double pointRadius;
    private readonly double slowMoveSpeed;
    private readonly double fastMoveSpeed;

    public IReadOnlyDictionary<TPoint, double> PointPositions => _pointPositions;


    public CycleMovingProcessor2(double slowMoveSpeed, double fastMoveSpeed, int maxPoints)
    {

        this.maxPoints = maxPoints;
        this.slowMoveSpeed = slowMoveSpeed;
        this.fastMoveSpeed = fastMoveSpeed;

        pointDistance = 1.0 / maxPoints;
        pointRadius = pointDistance / 2.0;
    }

    public bool TryAddPoint(TPoint point, double from, double to)
    {
        if (_orderedPoints.Count == 0)
        {
            double position = (from + to) / 2f;
            InsertPoint(point, position);

            return true;
        }
        else
        {
            bool success = TryFindPositionForInsertPointInRange(from, to, out double position);
            if (success) InsertPoint(point, position);

            return success;
        }

        void InsertPoint(TPoint point, double position)
        {
            int orderIndex = GetOrderIndexForInsertPoint(position);
            _orderedPoints.Insert(orderIndex, point);
            _pointPositions.Add(point, position);
        }
    }

    public void RemovePoint(TPoint point)
    {

    }



    public void Update(double deltaTime)
    {
        if (_orderedPoints.Count == 0) return;

        var headPoint = _orderedPoints[0];

        double slowMovement = slowMoveSpeed * deltaTime;
        double fastMovement = fastMoveSpeed * deltaTime;

        _pointPositions[headPoint] += slowMovement;

        for (int i = 1; i < _pointPositions.Count; i++)
        {
            var point = _orderedPoints[point];


        }


    }


    public void Clear()
    {

    }


    private bool TryFindPositionForInsertPointInRange
        (double minBorder, double maxBorder, out double result)
    {
        result = 0.0;

        double candidatePosition = minBorder + pointRadius;

        while (candidatePosition + pointRadius <= maxBorder)
        {
            bool moved = false;

            foreach (double otherPointPosition in _pointPositions.Values)
            {
                if (otherPointPosition < minBorder || otherPointPosition > maxBorder)
                    continue;

                double distance = Math.Abs(candidatePosition - otherPointPosition);

                if (distance < pointDistance)
                {
                    candidatePosition = otherPointPosition + pointDistance;
                    moved = true;
                    break;
                }
            }

            if (!moved)
            {
                result = candidatePosition;
                return true;
            }
        }

        return false;
    }


    private int GetOrderIndexForInsertPoint(double position)
    {
        if (_orderedPoints.Count == 0) return 0;

        double headPosition = _pointPositions[_orderedPoints[0]];
        double distanceToHead = GetForwardDistance(position, headPosition); 

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            double pointPosition = _pointPositions[_orderedPoints[i]];
            double otherDistanceToHead = GetForwardDistance(pointPosition, headPosition);

            if (distanceToHead < otherDistanceToHead) 
                return i;
        }

        return _orderedPoints.Count;
    }

    private double GetForwardDistance(double from, double to)
        => from < to ? to - from : 1.0 - from + to;



}
