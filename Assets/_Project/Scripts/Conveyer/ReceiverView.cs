using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UNodes;

public class ReceiverView : MonoBehaviour
{
    [SerializeField] private Transform _containerTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<SpriteRenderer> _insideBallRenderers;

    private readonly Holder<LevelController> levelController = new();

    private int _currentBalls = 0;
    private Tween _finishTween;

    private const int MAX_BALLS = 3;

    public ColorType ColorType { get; private set; }
    public bool IsFinished => _currentBalls >= MAX_BALLS;


    public void Initialize(ColorType colorType)
    {
        ColorType = colorType;

        var color = Configs.Colors.GetColor(colorType);

        _spriteRenderer.color = color;

        foreach (var insideBallRenderer in _insideBallRenderers)
            insideBallRenderer.color = color;
    }




    public void PutBall(BallView ball, Action onCompleted)
    {
        int placeIndex = _currentBalls;

        var targetTransform = _insideBallRenderers[placeIndex].transform;

        ball.AnimateMoveToBallTargetPlace(targetTransform, onCompleted: () =>
        {
            Pools.Balls.Return(ball);
            _insideBallRenderers[placeIndex].gameObject.SetActive(true);
        });

        _currentBalls++;

        if (_currentBalls == MAX_BALLS)
            AnimateFinish(() => Pools.Receivers.Return(this));
    }

    private void AnimateFinish(Action onCompleted)
    {
        const float DURATION = 0.5f;

        _containerTransform.DOScale(0f, DURATION).SetEase(Ease.OutQuad);
    }



}
