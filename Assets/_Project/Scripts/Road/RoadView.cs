using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RoadView : MonoBehaviour
{
    private class PlaceState
    {
        public SpriteRenderer spriteRenderer;
        public float moveProgress;
    }

    private class BallState
    {
        public BallView ball;
        public Transform realTransform;
    }

    [SerializeField] private List<float> _inputBallMoveProgressPoints;
    [SerializeField] private RoadTrigger _ballTrigger;
    [SerializeField] private SpriteRenderer _placeRendererPrefab;
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private Color _placeColor1;
    [SerializeField] private Color _placeColor2;

    private List<PlaceState> _placeStates = new();

    private const int PLACES_AMOUNT = 24;
    private const float BALL_NORM_DISTANCE = 1f / PLACES_AMOUNT;
    private const float CHECK_BALL_PROGRESS_DISTANCE = BALL_NORM_DISTANCE * 0.98f;
    private const float BALL_DISTANCE = 0.7f;
    private const float MOVE_SPEED = 0.23f;
    private const float FAST_MOVE_SPEED = 0.4f;

    private readonly CycleMovingAlgorithm<BallState> movingAlgorithm =
        new(MOVE_SPEED, FAST_MOVE_SPEED, BALL_NORM_DISTANCE);

    public RoadTrigger BallTrigger => _ballTrigger;



    private void Awake()
    {
        CreatePlaces();
    }

    public bool TryPutBall(BallView ball)
    {
        bool positionExists = false;
        float normPosition = 0f;

        foreach (var normPositionVariant in _inputBallMoveProgressPoints)
        {
            if (movingAlgorithm.AddPointIsAvailable(normPositionVariant))
            {
                positionExists = true;
                normPosition = normPositionVariant;
                break;
            };
        }

        if (!positionExists) return false;

        ball.SetPhysics(false);
        var realTransform = new GameObject("BallReal").transform;
        realTransform.SetParent(transform);
        realTransform.position = _spline.EvaluatePosition(normPosition);

        ball.AnimateMoveToRoadPlace(realTransform);

        var ballState = new BallState()
        {
            ball = ball,
            realTransform = realTransform,
        };

        movingAlgorithm.AddPoint(ballState, normPosition);

        return true;
    }



    public void CreatePlaces()
    {
        float progress = 0f;
        Color placeColor = _placeColor1;

        for (int i = 0, k = 1; i < PLACES_AMOUNT; i++, k++)
        {
            if (k >= 3)
            {
                placeColor = placeColor == _placeColor1 ? 
                    _placeColor2 : _placeColor1;

                k = 0;
            }

            Vector3 position = _spline.EvaluatePosition(progress);
            progress += BALL_NORM_DISTANCE;

            var placeRenderer = Instantiate(_placeRendererPrefab, transform);
            placeRenderer.transform.position = position;
            placeRenderer.color = placeColor;

            _placeStates.Add(new() 
            { 
                spriteRenderer = placeRenderer, 
                moveProgress = progress 
            });
        }
    }


    private void Update()
    {
        movingAlgorithm.Update(Time.deltaTime);

        UpdateMovePlaces();
        UpdateMoveBalls();
    }


    private void UpdateMovePlaces()
    {
        foreach (var placeState in _placeStates)
        {
            placeState.moveProgress =
                (placeState.moveProgress + Time.deltaTime * MOVE_SPEED) % 1f;

            Vector3 position = _spline.EvaluatePosition(placeState.moveProgress);
            placeState.spriteRenderer.transform.position = position;
        }
    }

    private void UpdateMoveBalls()
    {
        foreach (var ballState in movingAlgorithm.Points)
        {
            var ballNormPosition = movingAlgorithm.GetPosition(ballState);
            var position = _spline.EvaluatePosition(ballNormPosition);
            ballState.realTransform.position = position;
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        foreach (var progressPoint in _inputBallMoveProgressPoints)
        {
            var position = _spline.EvaluatePosition(progressPoint);
            Gizmos.DrawWireSphere(position, 0.1f);
        }

    }





}


