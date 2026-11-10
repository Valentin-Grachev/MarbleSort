using UnityEngine;
using UNodes;

public class Test : MonoBehaviour
{

    private readonly Holder<LevelController> levelController = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            levelController.Item.StartLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            levelController.Item.StartLevel(2);
        }
    }

}
