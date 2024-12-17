using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;
    public static GameObject _debugToolPref;

    /// <summary>
    /// ƒQ[ƒ€‹N“®‚ÉŒÄ‚Ño‚µ‚©‚©‚é
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
        _debugToolPref = Resources.Load("DebugToolCanvas") as GameObject;
    }
}