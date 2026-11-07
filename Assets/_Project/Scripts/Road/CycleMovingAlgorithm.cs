using System;
using System.Collections.Generic;

public class CycleMovingAlgorithm<T> where T : class
{
    private readonly Dictionary<T, float> _pointPositions = new();
    private readonly List<T> _orderedPoints = new();

    private readonly float distanceBetweenPoints;
    private readonly float slowMoveSpeed;
    private readonly float fastMoveSpeed;

    public CycleMovingAlgorithm(float slowMoveSpeed, float fastMoveSpeed, float distanceBetweenPoints)
    {
        if (slowMoveSpeed < 0f)
            throw new ArgumentOutOfRangeException(nameof(slowMoveSpeed));

        if (fastMoveSpeed < 0f)
            throw new ArgumentOutOfRangeException(nameof(fastMoveSpeed));

        if (distanceBetweenPoints < 0f || distanceBetweenPoints >= 1f)
            throw new ArgumentOutOfRangeException(nameof(distanceBetweenPoints));

        this.slowMoveSpeed = slowMoveSpeed;
        this.fastMoveSpeed = fastMoveSpeed;
        this.distanceBetweenPoints = distanceBetweenPoints;
    }

    public IReadOnlyList<T> Points => _orderedPoints;

    public void AddPoint(T pointComponent, float position)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        float normalizedPosition = Normalize(position);

        if (_pointPositions.ContainsKey(pointComponent))
        {
            _pointPositions[pointComponent] = normalizedPosition;
            return;
        }

        _pointPositions.Add(pointComponent, normalizedPosition);
        _orderedPoints.Add(pointComponent);
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
