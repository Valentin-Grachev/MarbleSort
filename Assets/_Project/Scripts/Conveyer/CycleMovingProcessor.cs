using System;
using System.Collections.Generic;

public class CycleMovingProcessor<T> where T : class
{
    private readonly Dictionary<T, float> _pointPositions = new();
    private readonly List<T> _orderedPoints = new();

    private readonly float distanceBetweenPoints;
    private readonly float slowMoveSpeed;
    private readonly float fastMoveSpeed;


    public IReadOnlyDictionary<T, float> PointPositions => _pointPositions;

    public CycleMovingProcessor(float slowMoveSpeed, float fastMoveSpeed, int maxPoints)
    {
        if (slowMoveSpeed < 0f)
            throw new ArgumentOutOfRangeException(nameof(slowMoveSpeed));

        if (fastMoveSpeed < 0f)
            throw new ArgumentOutOfRangeException(nameof(fastMoveSpeed));

        if (maxPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxPoints));

        this.slowMoveSpeed = slowMoveSpeed;
        this.fastMoveSpeed = fastMoveSpeed;
        distanceBetweenPoints = 1f / maxPoints;
    }

    public IReadOnlyList<T> Points => _orderedPoints;

    public bool AddPointIsAvailable(float position)
    {
        float normalizedPosition = Normalize(position);

        if (_orderedPoints.Count == 0)
            return true;

        int insertIndex = GetInsertIndex(normalizedPosition);
        return HasEnoughSpace(insertIndex, normalizedPosition);
    }

    public void AddPoint(T pointComponent, float position)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        if (_pointPositions.ContainsKey(pointComponent))
            return;

        float normalizedPosition = Normalize(position);

        if (_orderedPoints.Count == 0)
        {
            _pointPositions.Add(pointComponent, normalizedPosition);
            _orderedPoints.Add(pointComponent);
            return;
        }

        int insertIndex = GetInsertIndex(normalizedPosition);

        if (!HasEnoughSpace(insertIndex, normalizedPosition))
            return;

        _pointPositions.Add(pointComponent, normalizedPosition);
        _orderedPoints.Insert(insertIndex, pointComponent);
    }

    public void RemovePoint(T pointComponent)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        if (_pointPositions.Remove(pointComponent))
            _orderedPoints.Remove(pointComponent);
    }

    public float GetPosition(T pointComponent)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        return _pointPositions[pointComponent];
    }

    public void Update(float deltaTime)
    {
        if (deltaTime <= 0f || _orderedPoints.Count == 0)
            return;

        T leader = _orderedPoints[0];
        float leaderPosition = Normalize(_pointPositions[leader] + slowMoveSpeed * deltaTime);
        _pointPositions[leader] = leaderPosition;

        float previousPosition = leaderPosition;
        float previousSpeed = slowMoveSpeed;

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            T point = _orderedPoints[i];
            float currentPosition = _pointPositions[point];
            float distanceToPrevious = GetForwardDistance(currentPosition, previousPosition);

            float currentSpeed = distanceToPrevious > distanceBetweenPoints
                ? fastMoveSpeed
                : previousSpeed;

            float nextPosition = Normalize(currentPosition + currentSpeed * deltaTime);
            float nextDistanceToPrevious = GetForwardDistance(nextPosition, previousPosition);

            if (nextDistanceToPrevious < distanceBetweenPoints)
            {
                nextPosition = GetPositionBehind(previousPosition, distanceBetweenPoints);
                currentSpeed = previousSpeed;
            }

            _pointPositions[point] = nextPosition;
            previousPosition = nextPosition;
            previousSpeed = currentSpeed;
        }
    }

    private int GetInsertIndex(float position)
    {
        float leaderPosition = _pointPositions[_orderedPoints[0]];
        float newDistanceFromLeader = GetForwardDistance(position, leaderPosition);

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            float currentPosition = _pointPositions[_orderedPoints[i]];
            float currentDistanceFromLeader = GetForwardDistance(currentPosition, leaderPosition);

            if (newDistanceFromLeader < currentDistanceFromLeader)
                return i;
        }

        return _orderedPoints.Count;
    }

    private bool HasEnoughSpace(int insertIndex, float position)
    {
        float previousPosition = _pointPositions[_orderedPoints[insertIndex - 1]];
        float distanceToPrevious = GetForwardDistance(position, previousPosition);

        if (distanceToPrevious < distanceBetweenPoints)
            return false;

        if (insertIndex >= _orderedPoints.Count)
            return true;

        float nextPosition = _pointPositions[_orderedPoints[insertIndex]];
        float distanceToNext = GetForwardDistance(nextPosition, position);
        return distanceToNext >= distanceBetweenPoints;
    }

    private static float Normalize(float position)
    {
        position %= 1f;

        if (position < 0f)
            position += 1f;

        return position;
    }

    private static float GetForwardDistance(float fromPosition, float toPosition)
    {
        return Normalize(toPosition - fromPosition);
    }

    private static float GetPositionBehind(float position, float distance)
    {
        return Normalize(position - distance);
    }
}
