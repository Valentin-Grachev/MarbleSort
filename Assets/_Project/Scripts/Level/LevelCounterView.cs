using TMPro;
using UnityEngine;
using UNodes;

public class LevelCounterView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;

    private readonly Holder<LevelController> levelController = new();

    private void OnEnable()
    {
        levelController.Item.OnLevelStarted += Display;
    }

    private void OnDisable()
    {
        levelController.Item.OnLevelStarted -= Display;
    }

    private void Display()
    {
        _levelText.text = $"Level {levelController.Item.Level}";
    }


}
