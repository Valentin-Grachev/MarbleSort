using System;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator
{
    [Serializable] public struct ColorUnlock
    {
        public ColorType colorType;
        public int unlockLevel;
    }

    [Serializable] public struct GenerationConfig
    {
        public int startEmitters;
        public int addOneEmitterLevelsAmount;
        public int addEmitterLevelStep;
        public List<ColorUnlock> colorUnlocks;
    }



    

    private const int MIN_X = -4;
    private const int MAX_X = 3;
    private const int MIN_Y = 0;
    private const int MAX_Y = 6;
    private const int MAX_COLUMNS = 8;
    private const int MAX_ROWS = 7;
    private const int MAX_EMITTERS = MAX_COLUMNS * MAX_ROWS;
    private const int RECEIVER_QUEUES_COUNT = 4;
    private const int BALLS_PER_EMITTER = 9;
    private const int BALLS_PER_RECEIVER = 3;
    private const int RECEIVERS_PER_EMITTER = BALLS_PER_EMITTER / BALLS_PER_RECEIVER;

    private static readonly int[] COLUMN_ORDER = { 0, -1, 1, -2, 2, -3, 3, -4 };

    private int _baseEmittersCount = 3;
    private int _fastGrowthLevels = 6;
    private int _lateGrowthStepLevels = 2;
    private List<ColorUnlock> _colorUnlocks;


    public LevelGenerator(GenerationConfig config)
    {
        _baseEmittersCount = config.startEmitters;
        _fastGrowthLevels = config.addOneEmitterLevelsAmount;
        _lateGrowthStepLevels = config.addEmitterLevelStep;
        _colorUnlocks = config.colorUnlocks;
    }


    public LevelData GenerateLevelData( int level)
    {
        if (level < 1)
            throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be greater than or equal to 1.");

        var random = new System.Random(level * 9973);
        int emittersCount = GetEmittersCount(level);
        var positions = GenerateEmitterPositions(emittersCount);
        var revealOrderPositions = GenerateRevealOrder(positions, random);
        var availableColors = GetAvailableColors(level, emittersCount);
        var emitters = AssignEmitterColors(revealOrderPositions, availableColors, random);
        var receivers = GenerateReceivers(emitters, random);

        var levelData = new LevelData()
        {
            emitters = emitters,
            receivers = receivers,
        };

        ValidateLevel(levelData);
        return levelData;
    }

    private int GetEmittersCount(int level)
    {
        int levelsAfterFirst = Mathf.Max(0, level - 1);
        int fastGrowth = Mathf.Min(levelsAfterFirst, _fastGrowthLevels);
        int slowGrowthLevels = Mathf.Max(0, levelsAfterFirst - _fastGrowthLevels);
        int slowGrowth = Mathf.CeilToInt(slowGrowthLevels / (float)_lateGrowthStepLevels);

        int emittersCount = _baseEmittersCount + fastGrowth + slowGrowth;
        return Mathf.Clamp(emittersCount, 1, MAX_EMITTERS);
    }

    private List<Vector2Int> GenerateEmitterPositions(int emittersCount)
    {
        int usedColumns = Mathf.Min(MAX_COLUMNS, Mathf.CeilToInt(Mathf.Sqrt(emittersCount)));
        int baseHeight = emittersCount / usedColumns;
        int tallerColumns = emittersCount % usedColumns;
        int[] heights = new int[usedColumns];

        for (int i = 0; i < usedColumns; i++)
            heights[i] = baseHeight + (i < tallerColumns ? 1 : 0);

        var positions = new List<Vector2Int>(emittersCount);
        for (int columnIndex = 0; columnIndex < usedColumns; columnIndex++)
        {
            int x = COLUMN_ORDER[columnIndex];
            int height = heights[columnIndex];

            for (int y = MIN_Y; y < MIN_Y + height; y++)
                positions.Add(new Vector2Int(x, y));
        }

        return positions;
    }

    private List<Vector2Int> GenerateRevealOrder(List<Vector2Int> positions, System.Random random)
    {
        var columns = new List<List<Vector2Int>>(MAX_COLUMNS);
        for (int i = 0; i < MAX_COLUMNS; i++)
            columns.Add(new List<Vector2Int>());

        foreach (var position in positions)
            columns[position.x - MIN_X].Add(position);

        foreach (var column in columns)
            column.Sort((left, right) => left.y.CompareTo(right.y));

        var nextIndexes = new int[MAX_COLUMNS];
        var revealOrder = new List<Vector2Int>(positions.Count);
        int lastColumnIndex = -1;

        while (revealOrder.Count < positions.Count)
        {
            int chosenColumnIndex = GetNextColumnIndex(columns, nextIndexes, lastColumnIndex, random);
            revealOrder.Add(columns[chosenColumnIndex][nextIndexes[chosenColumnIndex]]);
            nextIndexes[chosenColumnIndex]++;
            lastColumnIndex = chosenColumnIndex;
        }

        return revealOrder;
    }

    private List<ColorType> GetAvailableColors(int level, int emittersCount)
    {
        if (_colorUnlocks == null || _colorUnlocks.Count == 0)
            throw new InvalidOperationException("LevelGenerator requires at least one color unlock setting.");

        var unlockedColors = new List<ColorType>();
        var usedColors = new HashSet<ColorType>();

        var sortedUnlocks = new List<ColorUnlock>(_colorUnlocks);
        sortedUnlocks.Sort((left, right) =>
        {
            int levelCompare = left.unlockLevel.CompareTo(right.unlockLevel);
            if (levelCompare != 0)
                return levelCompare;

            return left.colorType.CompareTo(right.colorType);
        });

        foreach (var unlock in sortedUnlocks)
        {
            if (unlock.unlockLevel > level)
                continue;

            if (usedColors.Add(unlock.colorType))
                unlockedColors.Add(unlock.colorType);
        }

        if (unlockedColors.Count == 0)
            throw new InvalidOperationException($"No colors are unlocked for level {level}.");

        int colorsToUse = Mathf.Min(unlockedColors.Count, emittersCount);
        if (colorsToUse < unlockedColors.Count)
            unlockedColors.RemoveRange(colorsToUse, unlockedColors.Count - colorsToUse);

        return unlockedColors;
    }

    private List<EmitterData> AssignEmitterColors(
        List<Vector2Int> positions,
        List<ColorType> availableColors,
        System.Random random)
    {
        var emitterColors = new List<ColorType>(positions.Count);

        foreach (var color in availableColors)
            emitterColors.Add(color);

        int nextColorIndex = 0;
        while (emitterColors.Count < positions.Count)
        {
            emitterColors.Add(availableColors[nextColorIndex]);
            nextColorIndex = (nextColorIndex + 1) % availableColors.Count;
        }

        Shuffle(emitterColors, random);

        var emitters = new List<EmitterData>(positions.Count);
        for (int i = 0; i < positions.Count; i++)
        {
            emitters.Add(new EmitterData()
            {
                gridPosition = positions[i],
                colorType = emitterColors[i],
            });
        }

        return emitters;
    }

    private List<ReceiverData> GenerateReceivers(List<EmitterData> emitters, System.Random random)
    {
        var receiverQueues = new List<List<ColorType>>(RECEIVER_QUEUES_COUNT);
        for (int i = 0; i < RECEIVER_QUEUES_COUNT; i++)
            receiverQueues.Add(new List<ColorType>());

        for (int emitterIndex = emitters.Count - 1; emitterIndex >= 0; emitterIndex--)
        {
            ColorType color = emitters[emitterIndex].colorType;
            int skippedQueue = GetQueueToSkip(receiverQueues, color, random);

            for (int queueIndex = 0; queueIndex < receiverQueues.Count; queueIndex++)
            {
                if (queueIndex == skippedQueue)
                    continue;

                // Prepending keeps the latest triplet on top and restores the previous solvable state after clearing it.
                receiverQueues[queueIndex].Insert(0, color);
            }
        }

        var receivers = new List<ReceiverData>(RECEIVER_QUEUES_COUNT);
        for (int i = 0; i < receiverQueues.Count; i++)
        {
            receivers.Add(new ReceiverData()
            {
                colorTypes = receiverQueues[i],
            });
        }

        return receivers;
    }

    private int GetQueueToSkip(List<List<ColorType>> receiverQueues, ColorType nextColor, System.Random random)
    {
        var nonMatchingQueueIndexes = new List<int>(receiverQueues.Count);
        for (int i = 0; i < receiverQueues.Count; i++)
        {
            if (receiverQueues[i].Count == 0 || receiverQueues[i][0] != nextColor)
                nonMatchingQueueIndexes.Add(i);
        }

        if (nonMatchingQueueIndexes.Count > 0)
            return GetLongestQueueIndex(receiverQueues, nonMatchingQueueIndexes, random);

        var allQueueIndexes = new List<int>(receiverQueues.Count);
        for (int i = 0; i < receiverQueues.Count; i++)
            allQueueIndexes.Add(i);

        return GetLongestQueueIndex(receiverQueues, allQueueIndexes, random);
    }

    private int GetLongestQueueIndex(
        List<List<ColorType>> receiverQueues,
        List<int> candidateIndexes,
        System.Random random)
    {
        int maxLength = int.MinValue;
        for (int i = 0; i < candidateIndexes.Count; i++)
            maxLength = Mathf.Max(maxLength, receiverQueues[candidateIndexes[i]].Count);

        var longestQueueIndexes = new List<int>(candidateIndexes.Count);
        for (int i = 0; i < candidateIndexes.Count; i++)
        {
            int candidateIndex = candidateIndexes[i];
            if (receiverQueues[candidateIndex].Count == maxLength)
                longestQueueIndexes.Add(candidateIndex);
        }

        return longestQueueIndexes[random.Next(longestQueueIndexes.Count)];
    }

    private void ValidateLevel(LevelData levelData)
    {
        if (levelData.emitters == null || levelData.emitters.Count == 0)
            throw new InvalidOperationException("Generated level must contain at least one emitter.");

        if (levelData.emitters.Count > MAX_EMITTERS)
            throw new InvalidOperationException($"Generated level contains too many emitters: {levelData.emitters.Count}.");

        var uniquePositions = new HashSet<Vector2Int>();
        var emitterColorCounts = new Dictionary<ColorType, int>();

        foreach (var emitter in levelData.emitters)
        {
            ValidatePosition(emitter.gridPosition);

            if (!uniquePositions.Add(emitter.gridPosition))
                throw new InvalidOperationException($"Duplicate emitter position detected: {emitter.gridPosition}.");

            IncrementCount(emitterColorCounts, emitter.colorType, BALLS_PER_EMITTER);
        }

        ValidateRevealOrder(levelData.emitters);

        if (levelData.receivers == null || levelData.receivers.Count != RECEIVER_QUEUES_COUNT)
            throw new InvalidOperationException($"Generated level must contain exactly {RECEIVER_QUEUES_COUNT} receiver queues.");

        int totalReceiverEntries = 0;
        var receiverColorCounts = new Dictionary<ColorType, int>();

        foreach (var receiver in levelData.receivers)
        {
            if (receiver.colorTypes == null)
                throw new InvalidOperationException("Receiver queue colorTypes must be initialized.");

            totalReceiverEntries += receiver.colorTypes.Count;

            foreach (var color in receiver.colorTypes)
                IncrementCount(receiverColorCounts, color, BALLS_PER_RECEIVER);
        }

        int expectedReceiverEntries = levelData.emitters.Count * RECEIVERS_PER_EMITTER;
        if (totalReceiverEntries != expectedReceiverEntries)
        {
            throw new InvalidOperationException(
                $"Receiver entries count mismatch. Expected {expectedReceiverEntries}, got {totalReceiverEntries}.");
        }

        foreach (var emitterColorCount in emitterColorCounts)
        {
            receiverColorCounts.TryGetValue(emitterColorCount.Key, out int receiverBalls);
            if (receiverBalls != emitterColorCount.Value)
            {
                throw new InvalidOperationException(
                    $"Color balance mismatch for {emitterColorCount.Key}. " +
                    $"Emitter balls: {emitterColorCount.Value}, receiver demand: {receiverBalls}.");
            }
        }
    }

    private void ValidatePosition(Vector2Int position)
    {
        if (position.x < MIN_X || position.x > MAX_X || position.y < MIN_Y || position.y > MAX_Y)
        {
            throw new InvalidOperationException(
                $"Emitter position {position} is outside the supported grid X[{MIN_X}..{MAX_X}] Y[{MIN_Y}..{MAX_Y}].");
        }
    }

    private void IncrementCount(Dictionary<ColorType, int> counts, ColorType color, int amount)
    {
        counts.TryGetValue(color, out int currentCount);
        counts[color] = currentCount + amount;
    }

    private int GetNextColumnIndex(
        List<List<Vector2Int>> columns,
        int[] nextIndexes,
        int lastColumnIndex,
        System.Random random)
    {
        int maxRemaining = 0;
        for (int i = 0; i < columns.Count; i++)
        {
            int remaining = columns[i].Count - nextIndexes[i];
            if (remaining > maxRemaining)
                maxRemaining = remaining;
        }

        var preferredColumns = new List<int>(columns.Count);
        for (int i = 0; i < columns.Count; i++)
        {
            int remaining = columns[i].Count - nextIndexes[i];
            if (remaining != maxRemaining || remaining <= 0)
                continue;

            if (i != lastColumnIndex)
                preferredColumns.Add(i);
        }

        if (preferredColumns.Count > 0)
            return preferredColumns[random.Next(preferredColumns.Count)];

        var fallbackColumns = new List<int>(columns.Count);
        for (int i = 0; i < columns.Count; i++)
        {
            int remaining = columns[i].Count - nextIndexes[i];
            if (remaining == maxRemaining && remaining > 0)
                fallbackColumns.Add(i);
        }

        return fallbackColumns[random.Next(fallbackColumns.Count)];
    }

    private void ValidateRevealOrder(List<EmitterData> emitters)
    {
        var remainingByColumn = new Dictionary<int, List<int>>();
        foreach (var emitter in emitters)
        {
            if (!remainingByColumn.TryGetValue(emitter.gridPosition.x, out var ys))
            {
                ys = new List<int>();
                remainingByColumn.Add(emitter.gridPosition.x, ys);
            }

            ys.Add(emitter.gridPosition.y);
        }

        foreach (var ys in remainingByColumn.Values)
            ys.Sort();

        for (int i = 0; i < emitters.Count; i++)
        {
            var emitter = emitters[i];
            var ys = remainingByColumn[emitter.gridPosition.x];

            if (ys.Count == 0 || ys[0] != emitter.gridPosition.y)
            {
                throw new InvalidOperationException(
                    $"Emitter order is not solvable with bottom-up column rule at step {i} for position {emitter.gridPosition}.");
            }

            ys.RemoveAt(0);
        }
    }

    private void Shuffle<T>(List<T> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }
}
