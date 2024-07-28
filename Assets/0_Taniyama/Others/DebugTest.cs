using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        
    }
    int i;
    int j;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR
            DebugEditor window = (DebugEditor)EditorWindow.GetWindow(typeof(DebugEditor), false,null,false);
            if (window != null)
            {
                i++;
                window.Log(i+"‰ñ‰Ÿ‚³‚ê‚½‚æ",DebugUser.Taniyama);
            }
#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
#if UNITY_EDITOR
            DebugEditor window = (DebugEditor)EditorWindow.GetWindow(typeof(DebugEditor), false, null, false);
            if (window != null)
            {
                j++;
                window.Log(j + "‰ñ‰Ÿ‚³‚ê‚½‚æ!", DebugUser.Taniyama);
            }
#endif
        }
    }
}
