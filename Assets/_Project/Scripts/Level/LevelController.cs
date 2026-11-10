using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public readonly List<ReceiverQueue> ReceiverQueues;
    public readonly ConveyerView Conveyer;

    private readonly Grid emitterGrid;



    public LevelController(Grid emitterGrid, ConveyerView conveyer, List<BallTrigger> receiverTriggers)
    {
        this.emitterGrid = emitterGrid;
        Conveyer = conveyer;

        ReceiverQueues = new List<ReceiverQueue>();
        foreach (var receiverTrigger in receiverTriggers)
            ReceiverQueues.Add(new ReceiverQueue(receiverTrigger));
    }


    public void StartLevel(int level)
    {
        var levelData = Configs.Levels.GetLevelData(level);

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
            ReceiverQueues[i].Initialize(receiverData.colorTypes);
        }
    }


}
