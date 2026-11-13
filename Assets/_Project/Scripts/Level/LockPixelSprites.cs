using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Resources/Lock Pixel Sprites", fileName = "LockPixelSprites")]
public class LockPixelSprites : ScriptableObject
{
    [field: SerializeField] public TileBase Center { get; private set; }
    [field: SerializeField] public TileBase HasTop { get; private set; }
    [field: SerializeField] public TileBase HasTopLeft { get; private set; }
    [field: SerializeField] public TileBase HasTopRight { get; private set; }
    [field: SerializeField] public TileBase HasRight { get; private set; }
    [field: SerializeField] public TileBase HasBottomRight { get; private set; }
    [field: SerializeField] public TileBase HasBottomLeft { get; private set; }
    [field: SerializeField] public TileBase HasBottom { get; private set; }
    [field: SerializeField] public TileBase HasLeft { get; private set; }
    [field: SerializeField] public TileBase HasTopLeftHole { get; private set; }
    [field: SerializeField] public TileBase HasTopRightHole { get; private set; }
    [field: SerializeField] public TileBase HasBottomRightHole { get; private set; }
    [field: SerializeField] public TileBase HasBottomLeftHole { get; private set; }




}
