using System;
using DG.Tweening;
using UnityEngine;




public class BallView : MonoBehaviour
{
    public enum State { Free, InConveyer, InReceiver }


    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public ColorType ColorType { get; private set; }


    public void Initialize(ColorType color)
    {
        ColorType = color;
        _spriteRenderer.color = Configs.Colors.GetColor(color);
    }

    public void MoveToConveyerCell(Transform cellTransform)
    {
        const float OFFSET_Y = 0.1f;

        float duration = Configs.Rules.BallMoveToConveyerDuration;

        _rigidbody.simulated = true;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.linearVelocity = Vector2.zero;

        transform.SetParent(cellTransform, worldPositionStays: true);
        transform.DOLocalMove(Vector3.up * OFFSET_Y, duration).SetEase(Ease.OutQuad);
    }

    public void MoveToReceiverCell(Transform cellTransform, Action onCompleted)
    {
        float duration = Configs.Rules.BallMoveToReceiverDuration;

        _rigidbody.simulated = false;

        transform.SetParent(cellTransform.transform, worldPositionStays: true);
        transform.DOLocalJump(Vector3.zero, 1, 1, duration).SetEase(Ease.OutQuad)
            .OnComplete(() => onCompleted?.Invoke());
    }


    public void SetPhysics(bool enable, Vector2 velocity = default)
    {
        _rigidbody.simulated = enable;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.linearVelocity = velocity;
    }



}
