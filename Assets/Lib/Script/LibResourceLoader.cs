using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;
    public static GameObject _debugToolPref;
    public static GameObject _uiEventSystem;

    /// <summary>
    /// �Q�[���N�����ɌĂяo��������
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
        _debugToolPref = Resources.Load("DebugToolCanvas") as GameObject;
        _uiEventSystem = Resources.Load("UI_EventSystem") as GameObject;
    }
}