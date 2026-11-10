using UnityEngine;
using UnityEngine.Splines;

public class ConveyerView : MonoBehaviour
{
    [SerializeField] private BallTrigger _inputBallTrigger;
    [SerializeField] private SpriteRenderer _placeRendererPrefab;
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private Color _placeColor1;
    [SerializeField] private Color _placeColor2;

    private CycleMovingProcessor<SpriteRenderer> _placeMovingProcessor;

    public SplineContainer MovingSpline => _spline;
    public BallTrigger InputBallTrigger => _inputBallTrigger;


    private void Start() => CreatePlaces();

    private void Update() => UpdateMovingPlaces();


    private void CreatePlaces()
    {
        var rules = Configs.Rules;

        _placeMovingProcessor = new(
            rules.ConveyerNormalizedSpeed,
            rules.ConveyerFastNormalizedSpeed,
            rules.ConveyerMaxBalls);

        float normalizedPosition = 0;
        float distanceBetweenPlaces = 1f / rules.ConveyerMaxBalls;
        for (int i = 0; i < rules.ConveyerMaxBalls; i++)
        {
            var placeRenderer = Instantiate(_placeRendererPrefab, transform);
            _placeMovingProcessor.AddPoint(placeRenderer, normalizedPosition);

            var color = i % 6 == 0 || i % 6 == 1 || i % 6 == 2 ?
                _placeColor1 : _placeColor2;

            placeRenderer.color = color;

            normalizedPosition += distanceBetweenPlaces;
        }
    }

    private void UpdateMovingPlaces()
    {
        _placeMovingProcessor.Update(Time.deltaTime);

        foreach (var placePoint in _placeMovingProcessor.PointPositions)
            placePoint.Key.transform.position = _spline.EvaluatePosition(placePoint.Value);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        foreach (var normalizedPosition in Configs.Rules.ConveyerInputNormalizedPositions)
        {
            var position = _spline.EvaluatePosition(normalizedPosition);
            Gizmos.DrawWireSphere(position, 0.1f);
        }
    }


}


