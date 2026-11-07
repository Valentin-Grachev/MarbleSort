using DG.Tweening;
using UnityEngine;

public class BallView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;

    private const float MOVE_TO_ROAD_PLACE_DURATION = 0.3f;


    public void AnimateMoveToRoadPlace(Transform roadPlace)
    {
        transform.SetParent(roadPlace.transform, worldPositionStays: true);

        transform.DOLocalMove(Vector3.zero, MOVE_TO_ROAD_PLACE_DURATION)
            .SetEase(Ease.OutQuad);
    }


    public void SetPhysics(bool enable)
    {
        _rigidbody.simulated = enable;
        _collider.enabled = enable;
    }


}
