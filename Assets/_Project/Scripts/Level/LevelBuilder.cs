using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
{
    private readonly Grid emitterGrid;
    private readonly List<ReceiverQueue> receiverQueues;


    public LevelBuilder(Grid emitterGrid, List<ReceiverQueue> receiverQueues)
    {
        this.emitterGrid = emitterGrid;
        this.receiverQueues = receiverQueues;
    }


    public void BuildLevel(LevelData levelData)
    {
        // <-- Create Emitters -->
        Dictionary<Vector2Int, EmitterView> createdEmitters = new();
        Pools.Emitters.Clear();

        foreach (var emitterData in levelData.emitters)
        {
            var emitter = Pools.Emitters.Get();

            var position = emitterData.gridPosition;
            emitter.transform.position = emitterGrid.GetCellCenterWorld((Vector3Int)position);

            bool isUnlocked = position.y == 0;

            emitter.Initialize(emitterData.colorType, isUnlocked);
            createdEmitters.Add(position, emitter);
        }

        // <-- Bind Unlockable Emitters -->
        foreach (var createdEmitter in createdEmitters)
        {
            var overPosition = new Vector2Int(createdEmitter.Key.x, createdEmitter.Key.y + 1);

            if (createdEmitters.ContainsKey(overPosition))
            {
                var unlockableEmitter = createdEmitters[overPosition];
                createdEmitter.Value.BindUnlockableEmitter(unlockableEmitter);
            }
        }


        // <-- Create Receivers -->
        for (int i = 0; i < levelData.receivers.Count; i++)
        {
            var receiverData = levelData.receivers[i];
            receiverQueues[i].Initialize(receiverData.colorTypes);
        }
    }

}
