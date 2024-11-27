using System;
using System.Text;

public static class LibFuncUtility
{
    /// <summary>
    /// ˆø”‚ªfalse‚Ì‚Ì‚İÀ{‚·‚éŠÖ”
    /// </summary>
    /// <param name="isDone">À{‚µ‚½‚±‚Æ‚ª‚ ‚é‚©</param>
    /// <param name="action">À{‚·‚éŠÖ”</param>
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