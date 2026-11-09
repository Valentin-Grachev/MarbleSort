using System;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WorldButton : MonoBehaviour
{
    public event Action OnClick;

    public void Click() => OnClick?.Invoke();


}
