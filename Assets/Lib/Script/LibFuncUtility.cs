using System;
using System.Text;

public static class LibFuncUtility
{
    /// <summary>
    /// ������false�̎��̂ݎ��{����֐�
    /// </summary>
    /// <param name="isDone">���{�������Ƃ����邩</param>
    /// <param name="action">���{����֐�</param>
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