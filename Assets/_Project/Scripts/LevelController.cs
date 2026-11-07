using UNodes;

public class LevelController : IUpdateHandler
{
    private readonly RoadView road;


    public LevelController(RoadView road)
    {
        this.road = road;
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
