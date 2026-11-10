using System;
using System.Collections.Generic;

public class CycleMovingProcessor<T> where T : class
{
    private readonly Dictionary<T, double> _positions =
        new Dictionary<T, double>();

    private readonly Dictionary<T, float> _pointPositions =
        new Dictionary<T, float>();

    private readonly List<T> _orderedPoints =
        new List<T>();

    private double[] _currentPositions = new double[0];
    private double[] _nextPositions = new double[0];

    private const double SPACE_EPSILON = 0.0000001;

    private readonly int maxPoints;
    private readonly double distanceBetweenPoints;
    private readonly double slowMoveSpeed;
    private readonly double fastMoveSpeed;

    public IReadOnlyDictionary<T, float> PointPositions => _pointPositions;

    public IReadOnlyList<T> Points => _orderedPoints;

    public CycleMovingProcessor(
        float slowMoveSpeed,
        float fastMoveSpeed,
        int maxPoints)
    {
        ValidateSpeed(slowMoveSpeed, nameof(slowMoveSpeed));
        ValidateSpeed(fastMoveSpeed, nameof(fastMoveSpeed));

        if (fastMoveSpeed < slowMoveSpeed)
            throw new ArgumentException("Fast speed must be >= slow speed.");

        if (maxPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxPoints));

        this.maxPoints = maxPoints;
        this.slowMoveSpeed = slowMoveSpeed;
        this.fastMoveSpeed = fastMoveSpeed;

        distanceBetweenPoints = 1.0 / maxPoints;
    }

    public bool AddPointIsAvailable(float position)
    {
        ValidatePosition(position);

        if (_orderedPoints.Count >= maxPoints)
            return false;

        double normalizedPosition = Normalize(position);

        if (_orderedPoints.Count == 0)
            return true;

        int insertIndex = GetInsertIndex(normalizedPosition);

        return HasEnoughSpace(insertIndex, normalizedPosition);
    }

    public void AddPoint(T pointComponent, float position)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        if (_positions.ContainsKey(pointComponent) ||
            _orderedPoints.Count >= maxPoints)
            return;

        ValidatePosition(position);

        double normalizedPosition = Normalize(position);

        if (_orderedPoints.Count == 0)
        {
            AddPosition(pointComponent, normalizedPosition);
            _orderedPoints.Add(pointComponent);
            return;
        }

        int insertIndex = GetInsertIndex(normalizedPosition);

        if (!HasEnoughSpace(insertIndex, normalizedPosition))
            return;

        AddPosition(pointComponent, normalizedPosition);
        _orderedPoints.Insert(insertIndex, pointComponent);
    }

    public void RemovePoint(T pointComponent)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        if (_positions.Remove(pointComponent))
        {
            _pointPositions.Remove(pointComponent);
            _orderedPoints.Remove(pointComponent);
        }
    }

    public float GetPosition(T pointComponent)
    {
        if (pointComponent == null)
            throw new ArgumentNullException(nameof(pointComponent));

        return _pointPositions[pointComponent];
    }

    public void Update(float deltaTime)
    {
        if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
            throw new ArgumentOutOfRangeException(nameof(deltaTime));

        if (deltaTime <= 0f || _orderedPoints.Count == 0)
            return;

        int count = _orderedPoints.Count;

        EnsureBufferCapacity(count);

        double dt = deltaTime;

        // Reconstruct unwrapped positions.
        _currentPositions[0] = _positions[_orderedPoints[0]];

        for (int i = 1; i < count; i++)
        {
            double gap = GetForwardDistance(
                _positions[_orderedPoints[i]],
                _positions[_orderedPoints[i - 1]]);

            _currentPositions[i] =
                _currentPositions[i - 1] - gap;
        }

        // Move leader.
        _nextPositions[0] =
            _currentPositions[0] + slowMoveSpeed * dt;

        // Move followers.
        for (int i = 1; i < count; i++)
        {
            double freePosition =
                _currentPositions[i] + fastMoveSpeed * dt;

            double maxAllowedPosition =
                _nextPositions[i - 1] - distanceBetweenPoints;

            _nextPositions[i] =
                Math.Min(freePosition, maxAllowedPosition);
        }

        // Save normalized positions.
        for (int i = 0; i < count; i++)
        {
            T point = _orderedPoints[i];

            double position = Normalize(_nextPositions[i]);

            _positions[point] = position;
            _pointPositions[point] = (float)position;
        }
    }

    private void AddPosition(T point, double position)
    {
        _positions.Add(point, position);
        _pointPositions.Add(point, (float)position);
    }

    private int GetInsertIndex(double position)
    {
        double leaderPosition = _positions[_orderedPoints[0]];

        double newDistance =
            GetForwardDistance(position, leaderPosition);

        for (int i = 1; i < _orderedPoints.Count; i++)
        {
            double currentDistance = GetForwardDistance(
                _positions[_orderedPoints[i]],
                leaderPosition);

            if (newDistance < currentDistance)
                return i;
        }

        return _orderedPoints.Count;
    }

    private bool HasEnoughSpace(int insertIndex, double position)
    {
        T previousPoint = _orderedPoints[insertIndex - 1];

        int nextIndex =
            insertIndex >= _orderedPoints.Count ? 0 : insertIndex;

        T nextPoint = _orderedPoints[nextIndex];

        double distanceToPrevious = GetForwardDistance(
            position,
            _positions[previousPoint]);

        double distanceToNext = GetForwardDistance(
            _positions[nextPoint],
            position);

        return distanceToPrevious + SPACE_EPSILON >= distanceBetweenPoints
            && distanceToNext + SPACE_EPSILON >= distanceBetweenPoints;
    }

    private void EnsureBufferCapacity(int capacity)
    {
        if (_currentPositions.Length >= capacity)
            return;

        Array.Resize(ref _currentPositions, capacity);
        Array.Resize(ref _nextPositions, capacity);
    }

    private static double Normalize(double position)
    {
        position %= 1.0;

        if (position < 0.0)
            position += 1.0;

        return position;
    }

    private static double GetForwardDistance(
        double fromPosition,
        double toPosition)
    {
        return Normalize(toPosition - fromPosition);
    }

    private static void ValidatePosition(float position)
    {
        if (float.IsNaN(position) || float.IsInfinity(position))
            throw new ArgumentOutOfRangeException(nameof(position));
    }

    private static void ValidateSpeed(float speed, string parameterName)
    {
        if (speed < 0f || float.IsNaN(speed) || float.IsInfinity(speed))
            throw new ArgumentOutOfRangeException(parameterName);
    }
}
