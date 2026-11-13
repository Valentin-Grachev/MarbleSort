using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder
{
    private readonly Grid emitterGrid;
    private readonly Tilemap lockPixelTilemap;
    private readonly List<ReceiverQueue> receiverQueues;
    private readonly LockPixelSprites lockPixelSprites;

    private readonly Vector3Int[] neighbors =
    {
        new(-1,  1, 0), // Top Left
        new( 0,  1, 0), // Top
        new( 1,  1, 0), // Top Right
        new( 1,  0, 0), // Right
        new( 1, -1, 0),  // Bottom Right
        new( 0, -1, 0), // Bottom
        new(-1, -1, 0), // Bottom Left
        new(-1,  0, 0), // Left
    };





    public LevelBuilder(Grid emitterGrid, Tilemap lockPixelTilemap, 
        LockPixelSprites lockPixelSprites, List<ReceiverQueue> receiverQueues)
    {
        this.emitterGrid = emitterGrid;
        this.lockPixelTilemap = lockPixelTilemap;
        this.receiverQueues = receiverQueues;
        this.lockPixelSprites = lockPixelSprites;
    }


    public void BuildLevel(LevelData levelData)
    {
        BuildLockPixels(levelData);

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


    private void BuildLockPixels(LevelData levelData)
    {
        const int MIN_X = -10, MAX_X = 9;
        const int MIN_Y = 0, MAX_Y = 16;

        var pixelPositions = new List<Vector3Int>();

        for (int x = MIN_X; x <= MAX_X; x++)
        {
            for (int y = MIN_Y; y < MAX_Y; y++)
            {
                int emitterX = x >= 0 ? x / 2 : (x - 1) / 2;
                int emitterY = y / 2;

                bool drawPixel = !levelData.emitters.Exists
                    ((emitterData) => emitterData.gridPosition == new Vector2Int(emitterX, emitterY));

                if (drawPixel)
                {
                    var position = new Vector3Int(x, y);
                    lockPixelTilemap.SetTile(position, lockPixelSprites.Center);
                    pixelPositions.Add(position);
                }
            }
        }

        
        foreach (var pixelPosition in pixelPositions)
        {
            int mask = GetNeighborMask(pixelPosition, pixelPositions);

            TileBase tile = mask switch
            {
                0b10000011 => lockPixelSprites.HasTopLeft,

                0b10001111 => lockPixelSprites.HasTop,
                0b10011111 => lockPixelSprites.HasTop,
                0b11001111 => lockPixelSprites.HasTop,

                0b00001110 => lockPixelSprites.HasTopRight,

                0b00111110 => lockPixelSprites.HasRight,
                0b00111111 => lockPixelSprites.HasRight,
                0b01111110 => lockPixelSprites.HasRight,

                0b00111000 => lockPixelSprites.HasBottomRight,

                0b11111000 => lockPixelSprites.HasBottom,
                0b11111100 => lockPixelSprites.HasBottom,
                0b11111001 => lockPixelSprites.HasBottom,

                0b11100000 => lockPixelSprites.HasBottomLeft,

                0b11100011 => lockPixelSprites.HasLeft,
                0b11110011 => lockPixelSprites.HasLeft,
                0b11100111 => lockPixelSprites.HasLeft,

                0b11111111 => lockPixelSprites.Center,

                0b11111110 => lockPixelSprites.HasTopLeftHole,
                0b11111011 => lockPixelSprites.HasTopRightHole,
                0b11101111 => lockPixelSprites.HasBottomRightHole,
                0b10111111 => lockPixelSprites.HasBottomLeftHole,

                _ => null,
            };

            if (tile == null)
            {
                Debug.Log(Convert.ToString(mask, 2));
            }


            lockPixelTilemap.SetTile(pixelPosition, tile);
        }
        

    }


    private int GetNeighborMask(Vector3Int position, List<Vector3Int> pixelPositions)
    {
        int mask = 0;

        for (int i = 0; i < neighbors.Length; i++)
        {
            Vector3Int neighborPosition = position + neighbors[i];

            if (pixelPositions.Contains(neighborPosition))
                mask |= 1 << i;
        }

        return mask;
    }

}
