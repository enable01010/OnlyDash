using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        LibDebug.ButtonLog("テスト",() => Debug.Log("ボタンが押されたよ"),DebugUser.Taniyama);
        LibDebug.ButtonLog("テスト", () => Debug.Log("ボタンが押されたよ"), DebugUser.Taniyama);
        LibDebug.ButtonLog("テスト", () => Debug.Log("ボタンが押されたよ"), DebugUser.Taniyama);
    }

    int i;
    int j;

    void Update()
    {
        LibDebug.LogIf("ifだよ", DebugUser.Matuoka ,() => Input.GetKey(KeyCode.A));

        if (Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR

            i++;
            LibDebug.Log(i + "Mtuoka\naaaa", DebugUser.Matuoka);

#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
#if UNITY_EDITOR
            j++;
            LibDebug.Log(j + "Taniyama", DebugUser.Taniyama);
#endif
        }
    }
}
