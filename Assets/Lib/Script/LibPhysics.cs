using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ray(Line)を出した際にDebug.Drawをするクラス
/// </summary>
public class LibPhysics
{

    /// <summary>
    /// Physics2D.Raycast 色と時間の指定なし
    /// </summary>
    /// <param name="origin"> 原点 </param>
    /// <param name="direction"> 方向 </param>
    /// <param name="distance"> 距離 </param>
    /// <param name="layerMask"> 特定のレイヤーのコライダーのみを判別 </param>
    /// <returns></returns>
    static public RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask=1)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Raycast(origin, direction, distance, layerMask);

#if UNITY_EDITOR
        Debug.DrawRay(origin, direction.normalized * distance);
#endif

        return answer;
    }


    /// <summary>
    /// Physics2D.Raycast 色と時間の指定あり 
    /// </summary>
    /// <param name="origin"> 原点 </param>
    /// <param name="direction"> 方向 </param>
    /// <param name="distance"> 距離 </param>
    /// <param name="layerMask"> 特定のレイヤーのコライダーのみを判別 </param>
    /// <param name="color"> ラインの色 </param>
    /// <param name="duration"> 消えるまでの時間(秒単位) (0の場合は1フレームのみ表示) </param>
    /// <returns></returns>
    static public RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask, Color color, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Raycast(origin, direction, distance, layerMask);

#if UNITY_EDITOR
        Debug.DrawRay(origin, direction.normalized * distance, color, duration);
#endif

        return answer;
    }


    /// <summary>
    /// Physics2D.Linecast 色と時間の指定なし
    /// </summary>
    /// <param name="start"> 開始地点 </param>
    /// <param name="end"> 終了地点 </param>
    /// <param name="layerMask"> 特定のレイヤーのコライダーのみを判別 </param>
    /// <returns></returns>
    static public RaycastHit2D Linecast2D(Vector2 start, Vector2 end, int layerMask)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Linecast(start, end, layerMask);

#if UNITY_EDITOR
        Debug.DrawLine(start, end);
#endif

        return answer;
    }


    /// <summary>
    /// Physics2D.Linecast 色と時間の指定あり
    /// </summary>
    /// <param name="start"> 開始地点 </param>
    /// <param name="end"> 終了地点 </param>
    /// <param name="layerMask"> 特定のレイヤーのコライダーのみを判別 </param>
    /// <param name="color"> ラインの色 </param>
    /// <param name="duration"> 消えるまでの時間(秒単位) (0の場合は1フレームのみ表示) </param>
    /// <returns></returns>
    static public RaycastHit2D Linecast2D(Vector2 start, Vector2 end, int layerMask, Color color, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Linecast(start, end, layerMask);

#if UNITY_EDITOR
        Debug.DrawLine(start, end, color, duration);
#endif

        return answer;
    }
}
