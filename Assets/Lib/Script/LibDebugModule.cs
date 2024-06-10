using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LibDebugModule : MonoBehaviour
{
    [SerializeField] public DebugUser outputs;
    public void Log(string text, DebugUser user)
    {
        int flag = (int)user & (int)outputs;
        if (flag != 0)
        {
            Debug.Log(user.ToString() + ":" + text);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LibDebugModule))]
public class LibDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LibDebugModule libDebug = target as LibDebugModule;

        libDebug.outputs =
            (DebugUser)EditorGUILayout.EnumMaskField("ï\é¶Ç∑ÇÈÉçÉO", libDebug.outputs);
    }
}
#endif
public class LibDebug
{
    private static LibDebug instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LibDebug();
            }
            return m_Instance;
        }
    }

    private static LibDebug m_Instance;
    LibDebugModule obj;

    private LibDebug()
    {
        GameObject pref = (GameObject)Resources.Load("Prefabs/DebugModule");
        obj = GameObject.Instantiate(pref).GetComponent<LibDebugModule>();
        GameObject.DontDestroyOnLoad(pref);
    }

    private void DellLog(string text, DebugUser user)
    {
        obj.Log(text, user);
    }


    public static void Log(string text, DebugUser user)
    {
#if UNITY_EDITOR

        instance.DellLog(text, user);
#endif
    }


}

public enum DebugUser
{
    Taniyama = 1 << 0,
    Matuoka = 1 << 1,
}
