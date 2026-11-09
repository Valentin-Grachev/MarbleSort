using System;
using DG.Tweening;
using UnityEngine;

public class BallView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public ColorType ColorType { get; private set; }


    public void Initialize(ColorType color)
    {
        ColorType = color;
        _spriteRenderer.color = Configs.Colors.GetColor(color);
    }


    public void AnimateMoveToConveyerCell(Transform cellTransform)
    {
        const float DURATION = 0.3f;

        transform.SetParent(cellTransform, worldPositionStays: true);
        transform.DOLocalMove(Vector3.zero, DURATION).SetEase(Ease.OutQuad);
    }

    public void AnimateMoveToBallTargetPlace(Transform placeTransform, Action onCompleted)
    {
        const float DURATION = 0.4f;

        transform.SetParent(placeTransform.transform, worldPositionStays: true);
        transform.DOLocalJump(Vector3.zero, 1, 1, DURATION).SetEase(Ease.OutQuad)
            .OnComplete(() => onCompleted?.Invoke());
    }



    public void SetPhysics(bool enable)
    {
        _rigidbody.simulated = enable;
        _collider.enabled = enable;
    }


}
