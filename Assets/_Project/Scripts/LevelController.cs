using UNodes;

public class LevelController : IUpdateHandler
{
    private readonly RoadView road;
    public readonly Pool<BallView> BallPool;


    public LevelController(RoadView road, BallView ballPrefab)
    {
        this.road = road;
        BallPool = new Pool<BallView>(ballPrefab);
    }



    void IUpdateHandler.Update()
    {
        if (road.BallTrigger.InsideBalls.Count > 0)
        {
            var ball = road.BallTrigger.InsideBalls[0];

            if (road.TryPutBall(ball))
                road.BallTrigger.RemoveBall(ball);
        }
    }



}
