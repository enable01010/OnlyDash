using UnityEngine;

public class LibResourceLoader
{
    public static GameObject bombFxPref;

    /// <summary>
    /// ゲーム起動時に呼び出しかかる
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}