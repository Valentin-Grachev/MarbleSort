using System;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    public event Action<BallView> OnBallEnter;
    //public event Action<BallView> OnBallExit;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ball")) 
            return;

        var ball = collision.gameObject.GetComponent<BallView>();
        OnBallEnter?.Invoke(ball);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ball"))
            return;

        var ball = collision.gameObject.GetComponent<BallView>();
        //OnBallExit?.Invoke(ball);
    }


}
