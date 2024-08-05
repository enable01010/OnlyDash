using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        //ボタンが生成されるよ
        LibDebug.ButtonLog("テスト1",() => Debug.Log("テスト1ボタンが押されたよ"),DebugUser.Taniyama);
        LibDebug.ButtonLog("テスト2", Test2, DebugUser.Matuoka);
    }

    private void Test2()
    {
        Debug.Log("ボタンが押されたよ");
    }

    private void Test3()
    {
        Debug.Log("テストが押されたよ3");
    }

    int i;
    int j;

    void Update()
    {
        //条件に合致した場合だけ呼ばれるよ
        LibDebug.LogIf("ifだよ", DebugUser.Matuoka ,() => Input.GetKey(KeyCode.A));
        LibDebug.LogIf("boolだよ", DebugUser.Taniyama, Input.GetKey(KeyCode.B));


        if (Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR

            //普通に呼ばれるヨ
            i++;
            LibDebug.Log(i + "ottotto", DebugUser.Matuoka);

#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
#if UNITY_EDITOR

            //普通に呼ばれるヨ
            j++;
            LibDebug.Log(j + "Taniyama", DebugUser.Taniyama);
#endif
        }
    }
}
