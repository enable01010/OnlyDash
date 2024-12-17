using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;
    public static GameObject _debugToolPref;

    /// <summary>
    /// �Q�[���N�����ɌĂяo��������
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
        _debugToolPref = Resources.Load("DebugToolCanvas") as GameObject;
    }
}