using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        LibDebug.ButtonLog("�e�X�g",() => Debug.Log("�{�^���������ꂽ��"),DebugUser.Taniyama);
        LibDebug.ButtonLog("�e�X�g", () => Debug.Log("�{�^���������ꂽ��"), DebugUser.Taniyama);
        LibDebug.ButtonLog("�e�X�g", () => Debug.Log("�{�^���������ꂽ��"), DebugUser.Taniyama);
    }

    int i;
    int j;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR

            i++;
            LibDebug.Log(i + "Mtuoka", DebugUser.Matuoka);

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
