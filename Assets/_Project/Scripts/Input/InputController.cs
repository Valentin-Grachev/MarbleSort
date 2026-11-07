using UnityEngine;
using UNodes;

public class InputController : IUpdateHandler
{
    private readonly Camera camera;


    public InputController(Camera camera)
    {
        this.camera = camera;
    }


    void IUpdateHandler.Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 position = camera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast
            (position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("WorldButton"));

        if (hit.collider != null)
        {
            var button = hit.collider.GetComponent<WorldButton>();
            button.Click();
        }
    }
}
