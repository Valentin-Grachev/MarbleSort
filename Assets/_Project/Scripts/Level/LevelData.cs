using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct EmitterData
{
    public Vector2Int gridPosition;
    public ColorType colorType;
}

[Serializable] public struct ReceiverData
{
    public List<ColorType> colorTypes;
}

[Serializable] public struct LevelData
{
    public List<EmitterData> emitters;
    public List<ReceiverData> receivers;
}
