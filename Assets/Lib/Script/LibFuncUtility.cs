using System;
using UnityEditor;

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
}