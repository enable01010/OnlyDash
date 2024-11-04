using System;
using UnityEditor;

public static class LibFuncUtility
{
    //引数がfalseの時のみ実施する関数
    public static void WhenFlaseDoAndReverse(ref bool isDone, Action action)
    {
        if(isDone == false)
        {
            isDone = true;
            action();
        }
    }
}