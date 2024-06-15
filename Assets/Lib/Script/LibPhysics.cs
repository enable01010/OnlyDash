using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ray(Line)���o�����ۂ�Debug.Draw������N���X
/// </summary>
public class LibPhysics
{
    /// <summary>
    /// Physics2D.Raycast
    /// </summary>
    /// <param name="origin"> ���_ </param>
    /// <param name="direction"> ���� </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Raycast(origin, direction, distance, layerMask);

#if UNITY_EDITOR
        Debug.DrawRay(origin, direction.normalized * distance, Color.red, duration);
#endif

        return answer;
    }



    /// <summary>
    /// Physics2D.Raycast (������Ray�ɕύX(����))
    /// </summary>
    /// <param name="ray"> ���C </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit2D Raycast2D(Ray ray, float distance, int layerMask = 1, float duration = 0.0f)
    {
        return Raycast2D(ray.origin, ray.direction, distance, layerMask, duration);
    }



    /// <summary>
    /// Physics2D.Linecast
    /// </summary>
    /// <param name="start"> �J�n�n�_ </param>
    /// <param name="end"> �I���n�_ </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit2D Linecast2D(Vector2 start, Vector2 end, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.Linecast(start, end, layerMask);

#if UNITY_EDITOR
        Debug.DrawLine(start, end, Color.red, duration);
#endif

        return answer;
    }



    /// <summary>
    /// Physics2D.CircleCast
    /// </summary>
    /// <param name="start"></param>
    /// <param name="radius"> �~�̔��a </param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit2D Circlecast2D(Vector2 start, float radius, Vector2 direction, float distance, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        //answer = Physics2D.CircleCast();

#if UNITY_EDITOR
        //Debug.DrawLine(start, end, Color.red, duration);
#endif

        return answer;
    }

    /// <summary>
    /// Physics.Raycast
    /// </summary>
    /// <param name="origin"> ���_ </param>
    /// <param name="direction"> ���� </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit answer = new RaycastHit();

        Physics.Raycast(origin, direction, out RaycastHit hitInfo, distance, layerMask);

        answer = hitInfo;

#if UNITY_EDITOR
        Debug.DrawRay(origin, direction.normalized * distance, Color.red, duration);
#endif

        return answer;
    }



    /// <summary>
    /// Physics.Raycast (������Ray�ɕύX)
    /// </summary>
    /// <param name="ray"> ���C </param>
    /// <param name="distance"> ���� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit Raycast(Ray ray, float distance, int layerMask = 1, float duration = 0.0f)
    {
        return Raycast(ray.origin, ray.direction, distance, layerMask, duration);
    }



    /// <summary>
    /// Physics.Linecast
    /// </summary>
    /// <param name="start"> �J�n�n�_ </param>
    /// <param name="end"> �I���n�_ </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit Linecast(Vector3 start, Vector3 end, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit answer = new RaycastHit();

        Physics.Linecast(start, end, out RaycastHit hitInfo, layerMask);

        answer = hitInfo;

#if UNITY_EDITOR
        Debug.DrawLine(start, end, Color.red, duration);
#endif

        return answer;
    }



    //static public Raycast2DBuilder BuildStart()
    //{
    //    return new Raycast2DBuilder();
    //}

    //public class Raycast2DBuilder
    //{
    //    Vector2 origin = Vector2.zero;
    //    Vector2 direction = Vector2.zero;
    //    float distance = 0.0f;
    //    int layerMask = 1;
    //    float duration = 0.0f;

    //    public Raycast2DBuilder SetOrigin(Vector2 origin) 
    //    { 
    //        this.origin = origin;
    //        return this;
    //    } 
    //    public Raycast2DBuilder SetDirection(Vector2 direction)
    //    { 
    //        this.direction = direction;
    //        return this;
    //    }
    //    public Raycast2DBuilder SetDistance(float distance)
    //    {
    //        this.distance = distance;
    //        return this;
    //    }
    //    public Raycast2DBuilder SetLayerMask(int layerMask)
    //    {
    //        this.layerMask = layerMask;
    //        return this;
    //    }
    //    public Raycast2DBuilder SetDuration(float duration)
    //    {
    //        this.duration = duration;
    //        return this;
    //    }

    //    public RaycastHit2D Raycast2D()
    //    {
    //        return LibPhysics.Raycast2D(origin, direction, distance, layerMask, duration);
    //    }
    //}

    // �g����
    //LibPhysics
    //        .BuildStart()
    //        .SetDirection()
    //        .SetDistance()
    //        .SetDuration()
    //        .SetOrigin()
    //        .SetLayerMask()
    //        .Raycast2D();
}
