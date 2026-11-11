using UnityEngine;
using UNodes;

public class Test : MonoBehaviour
{

    private readonly Holder<LevelController> levelController = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int level = Mathf.Max(1, levelController.Item.Level - 1);
            levelController.Item.StartLevel(level);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int level = levelController.Item.Level + 1;
            levelController.Item.StartLevel(level);
        }
    }

}
