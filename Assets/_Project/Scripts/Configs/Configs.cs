using UnityEngine;

public static class Configs
{
    public static ColorsConfig Colors => Resources.Load<ColorsConfig>("Colors");
    public static LevelsConfig Levels => Resources.Load<LevelsConfig>("Levels");
    public static RulesConfig Rules => Resources.Load<RulesConfig>("Rules");

}
