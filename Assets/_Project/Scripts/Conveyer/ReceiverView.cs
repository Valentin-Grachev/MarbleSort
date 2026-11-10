using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReceiverView : MonoBehaviour
{
    [SerializeField] private Transform _containerTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<SpriteRenderer> _insideBallRenderers;

    private Tween _finishTween;
    private int _currentBalls;
    private int _displayedBalls;

    private const int MAX_BALLS = 3;


    public ColorType ColorType { get; private set; }
    public bool IsFull => _currentBalls >= MAX_BALLS;
    public bool IsDisplayFull => _displayedBalls >= MAX_BALLS;
    public int BallsAmount => _currentBalls;


    public void Initialize(ColorType colorType)
    {
        ColorType = colorType;
        _currentBalls = 0;

        var color = Configs.Colors.GetColor(colorType);

        _spriteRenderer.color = color;

        foreach (var insideBallRenderer in _insideBallRenderers)
        {
            insideBallRenderer.enabled = false;
            insideBallRenderer.color = color;
        }
            
    }

    public void DisplayBallFullness(int balls)
    {
        _displayedBalls = balls;

        for (int i = 0; i < _insideBallRenderers.Count; i++)
            _insideBallRenderers[i].enabled = i < balls;
    }

    public Transform GetCurrentCellTransform()
        => _insideBallRenderers[_currentBalls].transform;

    public void IncreaseBallCount() => _currentBalls++;


    public void AnimateFinish(Action onCompleted = null)
    {
        float duration = Configs.Rules.ReceiverFinishDuration;
        _containerTransform.DOScale(0f, duration).SetEase(Ease.OutQuad);
    }



}
