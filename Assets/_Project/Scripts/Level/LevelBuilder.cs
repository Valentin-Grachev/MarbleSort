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
        Pools.Emitters.Clear();
        foreach (var emitterData in levelData.emitters)
        {
            var emitter = Pools.Emitters.Get();

            Vector3Int position = (Vector3Int)emitterData.gridPosition;
            emitter.transform.position = emitterGrid.GetCellCenterWorld(position);

            emitter.Initialize(emitterData.colorType);
        }

        // <-- Create Receivers -->
        for (int i = 0; i < levelData.receivers.Count; i++)
        {
            var receiverData = levelData.receivers[i];
            receiverQueues[i].Initialize(receiverData.colorTypes);
        }
    }

}
