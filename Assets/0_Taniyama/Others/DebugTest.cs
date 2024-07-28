using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
#if UNITY_EDITOR
            DebugEditor window = (DebugEditor)EditorWindow.GetWindow(typeof(DebugEditor));
            if (window != null)
            {
                window.Log("‰Ÿ‚³‚ê‚½‚æ",DebugUser.Taniyama);
            }
#endif
        }
    }
}
