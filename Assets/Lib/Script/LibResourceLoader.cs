using UnityEngine;

public class LibResourceLoader
{
    public static GameObject bombFxPref;

    /// <summary>
    /// �Q�[���N�����ɌĂяo��������
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        bombFxPref = Resources.Load("BombFxPref") as GameObject;
    }
}