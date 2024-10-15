using UnityEngine;

public class LibResourceLoader
{
    public static GameObject bombFxPref;

    /// <summary>
    /// ƒQ[ƒ€‹N“®‚ÉŒÄ‚Ño‚µ‚©‚©‚é
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}