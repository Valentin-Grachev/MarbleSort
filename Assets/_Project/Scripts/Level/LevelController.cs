using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    private List<ReceiverQueue> _receiverQueues;

    private readonly Grid emitterGrid;



    public LevelController(Grid emitterGrid, List<BallTrigger> receiverTriggers)
    {
        this.emitterGrid = emitterGrid; 

        _receiverQueues = new List<ReceiverQueue>();
        for (int i = 0; i < receiverTriggers.Count; i++)
        {
            var headPosition = receiverTriggers[i].transform.position;
            _receiverQueues.Add(new ReceiverQueue(headPosition));
        }

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
            _receiverQueues[i].Initialize(receiverData.colorTypes);
        }
    }


}
