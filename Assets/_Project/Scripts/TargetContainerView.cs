using System;
using UnityEngine;

public class TargetContainerView : MonoBehaviour
{
    private int _currentBalls = 0;

    private const int MAX_BALLS = 3;


    public bool IsFinished => _currentBalls >= MAX_BALLS;

    public void PutBall(BallView ball, Action onCompleted)
    {
        if (_currentBalls >= MAX_BALLS)
            throw new Exception("Trying to put the ball to a finished container!");

        _currentBalls++;
    }



}
