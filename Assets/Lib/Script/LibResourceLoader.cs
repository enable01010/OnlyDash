using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;

    /// <summary>
    /// ゲーム起動時に呼び出しかかる
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}