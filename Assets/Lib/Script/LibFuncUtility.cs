using System;
using UnityEditor;

public static class LibFuncUtility
{
    /// <summary>
    /// 引数がfalseの時のみ実施する関数
    /// </summary>
    /// <param name="isDone">実施したことがあるか</param>
    /// <param name="action">実施する関数</param>
    public static void WhenFlaseDoAndReverse(ref bool isDone, Action action)
    {
        if(isDone == false)
        {
            isDone = true;
            action();
        }
    }
}