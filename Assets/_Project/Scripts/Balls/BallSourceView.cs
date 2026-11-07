using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BallSourceView : MonoBehaviour
{
    [SerializeField] private WorldButton _worldButton;

    private const float UNPACK_WIDTH = 0.5f;
    private const float UNPACK_HEIGHT = 0.5f;
    private const float UNPACK_MOVE_Y = -0.5f;


    private readonly List<Vector2> unpackBallLocalPositions = new List<Vector2>()
    {
        new Vector2(-UNPACK_WIDTH, -UNPACK_HEIGHT),
        new Vector2(0f, 0f),
        new Vector2(UNPACK_WIDTH, UNPACK_HEIGHT),



    };




    private void Start()
    {
        _worldButton.OnClick += OnClick;
    }

    private void OnClick() => Unpack();



    public void Unpack()
    {
        DOTween.Sequence();



    }

    





}
