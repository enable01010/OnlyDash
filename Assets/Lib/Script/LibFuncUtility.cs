using System;
using System.Text;

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

    public static string TextFormatBuilder(string format, object arg0 = null, object arg1 = null, object arg2 = null)
    {
        var builder = new StringBuilder();
        builder.AppendFormat(format, arg0, arg1, arg2);
        return builder.ToString();
    }
}