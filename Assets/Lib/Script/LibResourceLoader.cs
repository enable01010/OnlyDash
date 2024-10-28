using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;

    /// <summary>
    /// ƒQ[ƒ€‹N“®‚ÉŒÄ‚Ño‚µ‚©‚©‚é
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}