using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UNodes;

public class EmitterView : MonoBehaviour
{
    [SerializeField] private WorldButton _worldButton;
    [SerializeField] private Transform _boxTransform;
    [SerializeField] private SpriteRenderer _boxRenderer;
    [SerializeField] private Sprite _fullBoxSprite;
    [SerializeField] private Sprite _emptyBoxSprite;

    private Tween _unpackBallTween;
    private Tween _boxTween;
    private ColorType _colorType;

    private const float UNPACK_WIDTH = 0.7f;
    private const float UNPACK_HEIGHT = 0.7f;
    private const float UNPACK_OFFSET_Y = 0.2f;
    private const float FROM_UNPACK_POSITION_PART = 0.4f;

    private const float BOX_CLICK_DURATION = 0.4f;
    private const float BOX_CLICK_TO_SCALE = 0.7f;
    private const float BOX_HIDE_DURATION = 0.4f;


    private readonly Holder<LevelController> levelController = new();

    private readonly List<Vector2> UNPACK_POSITIONS = new()
    {
        new Vector2(-UNPACK_WIDTH, UNPACK_OFFSET_Y - UNPACK_HEIGHT),
        new Vector2(0f, UNPACK_OFFSET_Y - UNPACK_HEIGHT),
        new Vector2(UNPACK_WIDTH, UNPACK_OFFSET_Y - UNPACK_HEIGHT),

        new Vector2(-UNPACK_WIDTH, UNPACK_OFFSET_Y),
        new Vector2(0f, UNPACK_OFFSET_Y),
        new Vector2(UNPACK_WIDTH, UNPACK_OFFSET_Y),

        new Vector2(-UNPACK_WIDTH, UNPACK_OFFSET_Y + UNPACK_HEIGHT),
        new Vector2(0f, UNPACK_OFFSET_Y + UNPACK_HEIGHT),
        new Vector2(UNPACK_WIDTH, UNPACK_OFFSET_Y + UNPACK_HEIGHT),
    };

    
    private void Start()
    {
        _worldButton.OnClick += OnClick;
    }


    private void OnDisable()
    {
        _unpackBallTween?.Kill();
        _boxTween?.Kill();
    }


    public void Initialize(ColorType colorType)
    {
        _colorType = colorType;
        _boxRenderer.sprite = _fullBoxSprite;
        _boxRenderer.color = Configs.Colors.GetColor(colorType);
        _boxTransform.transform.localScale = Vector3.one;
        _worldButton.SetInteractable(true);
    }



    private void OnClick() => EmitBalls();

    private void EmitBalls()
    {
        AnimateClickBox();

        _worldButton.SetInteractable(false);

        _unpackBallTween?.Kill();
        var sequence = DOTween.Sequence();

        for (int i = 0; i < UNPACK_POSITIONS.Count; i++)
        {
            int index = i;
            sequence.AppendCallback(() => AnimateSpawnBall(index));
            sequence.AppendInterval(Configs.Rules.EmitterBallEmitDelay);

            if (i == UNPACK_POSITIONS.Count - 1)
                sequence.JoinCallback(() => _boxRenderer.sprite = _emptyBoxSprite);
        }

        sequence.OnComplete(() =>
        {
            AnimateHideBox(onCompleted: () => Pools.Emitters.Return(this));
        });

        _unpackBallTween = sequence;
    }

    private Tween AnimateSpawnBall(int index)
    {
        BallView ball = Pools.Balls.Get();

        var targetPosition = UNPACK_POSITIONS[index];

        ball.Initialize(_colorType);
        ball.transform.SetParent(transform, worldPositionStays: false);
        ball.transform.localPosition = targetPosition * FROM_UNPACK_POSITION_PART;
        ball.transform.localScale = Vector3.zero;

        ball.SetPhysics(false);

        float duration = Configs.Rules.EmitterOneBallEmitDuration;

        var scaleTween = ball.transform.DOScale(1f, duration).SetEase(Ease.OutQuad);

        var moveTween = ball.transform.DOLocalJump(targetPosition, 0.3f, 1, duration)
            .SetEase(Ease.InFlash).OnComplete(() =>
            {
                ball.SetPhysics(true, Vector2.up * Configs.Rules.EmitterBallStartVelocityY);
                ball.transform.SetParent(Pools.Balls.Container);
            });

        return DOTween.Sequence().Append(scaleTween).Join(moveTween);
    }


    private void AnimateClickBox()
    {
        _boxTween?.Kill();
        var sequence = DOTween.Sequence();

        sequence.Append(_boxTransform.DOScale(BOX_CLICK_TO_SCALE, BOX_CLICK_DURATION / 2f))
            .SetEase(Ease.OutQuad);

        sequence.Append(_boxTransform.DOScale(1f, BOX_CLICK_DURATION / 2f))
            .SetEase(Ease.InQuad);

        _boxTween = sequence;
    }

    private void AnimateHideBox(Action onCompleted)
    {
        _boxTween?.Kill();
        _boxTween = _boxTransform.DOScale(0f, BOX_HIDE_DURATION)
            .SetEase(Ease.OutQuad);

        _boxTween.OnComplete(() => onCompleted?.Invoke());
    }




}
