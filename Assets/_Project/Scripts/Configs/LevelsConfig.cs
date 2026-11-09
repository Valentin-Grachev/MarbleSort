using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Levels", fileName = "Levels")]
public class LevelsConfig : ScriptableObject
{
    [SerializeField] private List<LevelData> _levels;

    public LevelData GetLevelData(int level) => _levels[level - 1];


}
