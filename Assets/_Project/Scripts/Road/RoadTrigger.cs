using System.Collections.Generic;
using UnityEngine;

public class RoadTrigger : MonoBehaviour
{
    private List<BallView> _insideBalls = new();

    public IReadOnlyList<BallView> InsideBalls => _insideBalls;


    public void RemoveBall(BallView ball) => _insideBalls.Remove(ball);


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ball")) return;

        var ball = collision.gameObject.GetComponent<BallView>();
        _insideBalls.Add(ball);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ball")) return;

        var ball = collision.gameObject.GetComponent<BallView>();
        _insideBalls.Remove(ball);
    }



}
