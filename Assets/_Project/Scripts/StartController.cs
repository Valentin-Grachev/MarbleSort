using UNodes;

public class StartController : IStartHandler
{
    private readonly LevelController levelController;


    public StartController(LevelController levelController)
    {
        this.levelController = levelController;
    }



    void IStartHandler.Start()
    {
        levelController.StartLevel(1);
    }
}
