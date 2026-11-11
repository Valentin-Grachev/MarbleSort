using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Levels", fileName = "Levels")]
public class LevelsConfig : ScriptableObject
{
    [SerializeField] private LevelGenerator.GenerationConfig _generationConfig;
    [SerializeField] private Vector2Int _generateFromToLevel;
    [SerializeField] private List<LevelData> _levels;

    public LevelData GetLevelData(int level) => _levels[level - 1];


    [ContextMenu("Generate")]
    private void GenerateLevels()
    {
        EditorUtility.SetDirty(this);

        var generator = new LevelGenerator(_generationConfig);

        for (int level = _generateFromToLevel.x; level <= _generateFromToLevel.y; level++)
        {
            int index = level - 1;
            var levelData = generator.GenerateLevelData(level);

            if (index < _levels.Count) _levels[index] = levelData;
            else _levels.Add(levelData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

}
