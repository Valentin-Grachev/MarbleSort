using System;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WorldButton : MonoBehaviour
{
    private Collider2D _collider;


    public event Action OnClick;

    public void Click() => OnClick?.Invoke();

    public void SetInteractable(bool interactable)
    {
        _collider ??= GetComponent<Collider2D>();
        _collider.enabled = interactable;
    }

}
