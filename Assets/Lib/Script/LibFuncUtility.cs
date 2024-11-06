using System;
using UnityEditor;

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
}