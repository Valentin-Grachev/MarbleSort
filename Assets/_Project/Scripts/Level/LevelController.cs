using System;

public class LevelController
{
    public event Action OnLevelStarted;


    public int Level { get; private set; }



    public void StartLevel(int level)
    {
        Level = level;
        OnLevelStarted?.Invoke();
    }

    public void Lose()
    {

    }

    public void Win()
    {

    }




}
