using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ray(Line)���o�����ۂ�Debug.Draw������N���X
/// </summary>
public class LibPhysics
{

    /// <summary>
    /// Physics2D.Raycast �F�Ǝ��Ԃ̎w��Ȃ�
    /// </summary>
    /// <param name="origin"> ���_ </param>
    /// <param name="direction"> ���� </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� </param>
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
    /// Physics2D.Raycast �F�Ǝ��Ԃ̎w�肠�� 
    /// </summary>
    /// <param name="origin"> ���_ </param>
    /// <param name="direction"> ���� </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� </param>
    /// <param name="color"> ���C���̐F </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
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
    /// Physics2D.Linecast �F�Ǝ��Ԃ̎w��Ȃ�
    /// </summary>
    /// <param name="start"> �J�n�n�_ </param>
    /// <param name="end"> �I���n�_ </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� </param>
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
    /// Physics2D.Linecast �F�Ǝ��Ԃ̎w�肠��
    /// </summary>
    /// <param name="start"> �J�n�n�_ </param>
    /// <param name="end"> �I���n�_ </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� </param>
    /// <param name="color"> ���C���̐F </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
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
