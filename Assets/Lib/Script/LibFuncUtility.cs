using System;
using UnityEditor;

public static class LibFuncUtility
{
    //������false�̎��̂ݎ��{����֐�
    public static void WhenFlaseDoAndReverse(ref bool isDone, Action action)
    {
        if(isDone == false)
        {
            isDone = true;
            action();
        }
    }
}