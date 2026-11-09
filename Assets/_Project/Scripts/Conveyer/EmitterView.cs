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
    private const float UNPACK_HEIGHT = 0.5f;
    private const float UNPACK_ROW_DELAY = 0.15f;
    private const float UNPACK_MOVE_DURATION = 0.2f;

    private const float BOX_CLICK_DURATION = 0.4f;
    private const float BOX_CLICK_TO_SCALE = 0.7f;
    private const float BOX_HIDE_DURATION = 0.4f;


    private const int BALLS_PER_ROW = 3;
    private const int ROWS_AMOUNT = 3;


    private readonly Holder<LevelController> levelController = new();

    private readonly List<Vector2> UNPACK_POSITIONS = new()
    {
        new Vector2(-UNPACK_WIDTH, -UNPACK_HEIGHT),
        new Vector2(0f, -UNPACK_HEIGHT),
        new Vector2(UNPACK_WIDTH, -UNPACK_HEIGHT),

        new Vector2(-UNPACK_WIDTH, 0f),
        new Vector2(0f, 0f),
        new Vector2(UNPACK_WIDTH, 0f),

        new Vector2(-UNPACK_WIDTH, UNPACK_HEIGHT),
        new Vector2(0f, UNPACK_HEIGHT),
        new Vector2(UNPACK_WIDTH, UNPACK_HEIGHT),
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
    }



    private void OnClick() => Unpack();

    private void Unpack()
    {
        AnimateClickBox();

        _unpackBallTween?.Kill();
        var sequence = DOTween.Sequence();

        for (int i = 0; i < ROWS_AMOUNT; i++)
        {
            sequence.Append(AnimateSpawnBallRow(i));

            if (i == ROWS_AMOUNT - 1)
                sequence.JoinCallback(() => _boxRenderer.sprite = _emptyBoxSprite);
        }

        sequence.OnComplete(() =>
        {
            AnimateHideBox(onCompleted: () => Pools.Emitters.Return(this));
        });

        _unpackBallTween = sequence;
    }

    private Tween AnimateSpawnBallRow(int rowIndex)
    {
        var sequence = DOTween.Sequence();

        for (int i = 0; i < BALLS_PER_ROW; i++)
        {
            BallView ball = Pools.Balls.Get();

            ball.Initialize(_colorType);
            ball.transform.SetParent(transform, worldPositionStays: false);
            ball.transform.localPosition = Vector3.zero;
            ball.transform.localScale = Vector3.zero;

            ball.SetPhysics(false);

            var targetPosition = UNPACK_POSITIONS[rowIndex * BALLS_PER_ROW + i];

            var scaleTween = ball.transform.DOScale(1f, UNPACK_MOVE_DURATION)
                .SetEase(Ease.OutQuad);

            var moveTween = ball.transform.DOLocalMove(targetPosition, UNPACK_MOVE_DURATION)
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    ball.SetPhysics(true);
                    ball.transform.SetParent(Pools.Balls.Container);
                });

            sequence.Join(moveTween);
            sequence.Join(scaleTween);
        }

        return sequence;
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
