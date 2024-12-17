using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugTest : MonoBehaviour
{

    void Start()
    {
        //�{�^��������������
        LibDebug.ButtonLog("�e�X�g1",() => Debug.Log("�e�X�g1�{�^���������ꂽ��"),DebugUser.Taniyama);
        LibDebug.ButtonLog("�e�X�g2", Test2, DebugUser.Matuoka);
    }

    private void Test2()
    {
        Debug.Log("�{�^���������ꂽ��");
    }

    private void Test3()
    {
        Debug.Log("�e�X�g�������ꂽ��3");
    }

    int i;
    int j;

    void Update()
    {
        //�����ɍ��v�����ꍇ�����Ă΂���
        LibDebug.LogIf("if����", DebugUser.Matuoka ,() => Input.GetKey(KeyCode.A));
        LibDebug.LogIf("bool����", DebugUser.Taniyama, Input.GetKey(KeyCode.B));


        if (Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR

            //���ʂɌĂ΂�郈
            i++;
            LibDebug.Log(i + "ottotto", DebugUser.Matuoka);

#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
#if UNITY_EDITOR

            //���ʂɌĂ΂�郈
            j++;
            LibDebug.Log(j + "Taniyama", DebugUser.Taniyama);
#endif
        }
    }
}
