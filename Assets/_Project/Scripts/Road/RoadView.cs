using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RoadView : MonoBehaviour
{
    private class PlaceRendererState
    {
        public SpriteRenderer spriteRenderer;
        public float moveProgress;
    }

    private class BallPlaceState
    {
        public RoadBallPlace ballPlace;
        public float moveProgress;
    }



    [SerializeField] private SpriteRenderer _placeRendererPrefab;
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private Color _placeColor1;
    [SerializeField] private Color _placeColor2;

    private List<PlaceRendererState> _placeRendererStates = new();
    private List<BallPlaceState> _ballPlaceStates = new();
    private BallView _mainBall = null;

    private const int PLACES_AMOUNT = 24;
    private const float BETWEEN_DISTANCE = 1f / PLACES_AMOUNT;
    private const float MOVE_SPEED = 0.23f;
    private const float FAST_MOVE_SPEED = 0.4f;



    private void Awake()
    {
        CreatePlaces();
    }


    public void PutBall(BallView ball)
    {
        _mainBall ??= ball;



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
            progress += BETWEEN_DISTANCE;

            var placeRenderer = Instantiate(_placeRendererPrefab, transform);
            placeRenderer.transform.position = position;
            placeRenderer.color = placeColor;

            _placeRendererStates.Add(new() 
            { 
                spriteRenderer = placeRenderer, 
                moveProgress = progress 
            });
        }
    }


    private void Update()
    {
        UpdateMovePlaces();
    }


    private void UpdateMovePlaces()
    {
        foreach (var placeData in _placeRendererStates)
        {
            placeData.moveProgress =
                (placeData.moveProgress + Time.deltaTime * MOVE_SPEED) % 1f;

            Vector3 position = _spline.EvaluatePosition(placeData.moveProgress);
            placeData.spriteRenderer.transform.position = position;

        }
    }




    private static class AI
    {
        public static void AddBallToRoad(RoadView road, BallView ball)
        {
            
        }

        public static void RemoveBallFromRoad(RoadView road, BallView ball)
        {

        }

        public static void UpdateMoveBalls()
        {

        }
    }



}


