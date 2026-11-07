using DG.Tweening;
using UnityEngine;

public class RoadBallPlace : MonoBehaviour
{
    private BallView _ball;

    private const float PUT_DURATION = 0.3f;


    public bool HasBall => _ball != null;
    
    public void PutBall(BallView ball)
    {
        ball.transform.SetParent(transform, worldPositionStays: true);
        _ball = ball;

        ball.transform.DOLocalMove(Vector3.zero, PUT_DURATION).SetEase(Ease.OutQuad);
    }

    public void SetFree() => _ball = null;


}
