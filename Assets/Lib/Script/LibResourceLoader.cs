using UnityEngine;

public class LibResourceLoader
{
    public static GameObject _bombFxPref;

    /// <summary>
    /// �Q�[���N�����ɌĂяo��������
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        _bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}