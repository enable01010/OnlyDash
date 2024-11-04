using System;
using UnityEditor;

public static class LibFuncUtility
{
    //ˆø”‚ªfalse‚Ì‚Ì‚İÀ{‚·‚éŠÖ”
    public static void WhenFlaseDoAndReverse(ref bool isDone, Action action)
    {
        if(isDone == false)
        {
            isDone = true;
            action();
        }
    }
}