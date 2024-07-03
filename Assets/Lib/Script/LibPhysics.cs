using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ray(Line)���o�����ۂ�Debug.Draw������N���X
/// </summary>
public static class LibPhysics
{
    #region Raycast2D

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
        
        if (answer == true)
        {
            Debug.DrawRay(origin, direction.normalized * distance, Color.green, duration);
        }
        else
        {
            Debug.DrawRay(origin, direction.normalized * distance, Color.red, duration);
        }

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

        if (answer == true)
        {
            Debug.DrawLine(start, end, Color.green, duration);
        }
        else
        {
            Debug.DrawLine(start, end, Color.red, duration);
        }

#endif

        return answer;
    }



    /// <summary>
    /// Physics2D.CircleCast
    /// </summary>
    /// <param name="origin"> �~�̒��S </param>
    /// <param name="radius"> �~�̔��a </param>
    /// <param name="direction"> �~��L�΂����� </param>
    /// <param name="distance"> �~��L�΂����� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit2D CircleCast2D(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit2D answer = new RaycastHit2D();

        answer = Physics2D.CircleCast(origin, radius, direction, distance, layerMask);

#if UNITY_EDITOR

        if (answer == true)
        {
            DrawCircleLine(origin, radius, direction, distance, duration, Color.green);
        }
        else
        {
            DrawCircleLine(origin, radius, direction, distance, duration, Color.red);
        }

#endif

        return answer;
    }



    #endregion
    
    #region Raycast3D

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
        RaycastHit answer;

        Physics.Raycast(origin, direction, out answer, distance, layerMask);

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
        RaycastHit answer;

        Physics.Linecast(start, end, out answer, layerMask);

#if UNITY_EDITOR
        Debug.DrawLine(start, end, Color.red, duration);
#endif

        return answer;
    }



    /// <summary>
    /// Physics.SphereCast
    /// </summary>
    /// <param name="origin"> ���̒��S </param>
    /// <param name="radius"> ���̔��a </param>
    /// <param name="direction"> ����L�΂����� </param>
    /// <param name="distance"> ����L�΂����� </param>
    /// <param name="layerMask"> ����̃��C���[�̃R���C�_�[�݂̂𔻕� (1�̏ꍇ��Default�̂�) </param>
    /// <param name="duration"> ������܂ł̎���(�b�P��) (0�̏ꍇ��1�t���[���̂ݕ\��) </param>
    /// <returns></returns>
    static public RaycastHit SphereCast(Vector3 origin, float radius, Vector3 direction, float distance, int layerMask = 1, float duration = 0.0f)
    {
        RaycastHit answer;

        Physics.SphereCast(origin, radius, direction, out answer, distance, layerMask);

#if UNITY_EDITOR

        if (answer.transform != null)
        {
            DrawSphereLine(origin, radius, direction, distance, duration, Color.green);
        }
        else
        {
            DrawSphereLine(origin, radius, direction, distance, duration, Color.red);
        }
        
#endif

        return answer;
    }



    #endregion
    
    #region Draw

    // CircleCast��Draw
    static private void DrawCircleLine(Vector2 origin, float radius, Vector2 direction, float distance, float duration, Color drawColor)
    {
        // Ray�̐F
        Color color = drawColor;

        // origin�̉~�̏\��
        Vector2 right = origin + new Vector2(radius, 0);
        Vector2 left = origin + new Vector2(-radius, 0);
        Vector2 up = origin + new Vector2(0, radius);
        Vector2 down = origin + new Vector2(0, -radius);

        Debug.DrawLine(right, left, color, duration);
        Debug.DrawLine(up, down, color, duration);


        // offset�̉~�̏\��
        Vector2 offset = direction.normalized * distance;

        Debug.DrawLine(right + offset, left + offset, color, duration);
        Debug.DrawLine(up + offset, down + offset, color, duration);


        // �~�̎�
        const int VERTEX = 64;// ���_�̐�
        float angle = 2 * Mathf.PI / VERTEX;

        for (int i = 0; i < VERTEX; i++)
        {
            // origin�̉~�̎�
            Vector2 temp1 = origin + new Vector2(radius * Mathf.Cos(angle * i), radius * Mathf.Sin(angle * i));
            Vector2 temp2 = origin + new Vector2(radius * Mathf.Cos(angle * (i + 1)), radius * Mathf.Sin(angle * (i + 1)));

            Debug.DrawLine(temp1, temp2, color, duration);


            // offset�̉~�̎�
            Vector2 temp3 = offset + temp1;
            Vector2 temp4 = offset + temp2;

            Debug.DrawLine(temp3, temp4, color, duration);
        }


        // start�̉~��offset�̉~���Ȃ�
        Vector2 nor = Vector2.Perpendicular(offset).normalized;// �@���x�N�g��(normalized����Ă��Ȃ�)

        Debug.DrawLine(origin + nor * radius, offset + origin + nor * radius, color, duration);
        Debug.DrawLine(origin - nor * radius, offset + origin - nor * radius, color, duration);
    }

    // SphereCast��Draw
    static private void DrawSphereLine(Vector3 origin, float radius, Vector3 direction, float distance, float duration, Color drawColor)
    {
        Color color = drawColor;

        // offset
        Vector3 offset = direction.normalized * distance;


        // x = rcos��cos��
        // y = rsin��      �c��
        // x = rcos��sin��

        float theta;// �ƁF���ʊp(�o�x) 0 <= �� <= 360
        float phi;  // �ӁF�p�@(�ܓx) 0 <= �� <= 180


        // ���̎�
        int VERTEX = 64;// ���_�̐�
        int halfVertex = VERTEX / 2;
        float angle = 2 * Mathf.PI / halfVertex;

        int surfaceNum = 64;// �o��(�c�̐�)�̐�
        float angle2 = 2 * Mathf.PI / surfaceNum;

        for (int i = 0; i < surfaceNum; i++)
        {
            theta = angle2 * i;

            for (int j = 0; j < halfVertex; j++)
            {
                phi = angle * j;

                float phiNext = angle * (j + 1);

                // origin�̋��̎�
                Vector3 vertex1 = origin;
                Vector3 vertex2 = origin;

                vertex1.x += radius * Mathf.Cos(phi) * Mathf.Cos(theta);
                vertex1.y += radius * Mathf.Sin(phi);
                vertex1.z += radius * Mathf.Cos(phi) * Mathf.Sin(theta);

                vertex2.x += radius * Mathf.Cos(phiNext) * Mathf.Cos(theta);
                vertex2.y += radius * Mathf.Sin(phiNext);
                vertex2.z += radius * Mathf.Cos(phiNext) * Mathf.Sin(theta);

                Debug.DrawLine(vertex1, vertex2, color, duration);


                // offset�̋��̎�
                Vector3 vertex3 = offset + vertex1;
                Vector3 vertex4 = offset + vertex2;

                Debug.DrawLine(vertex3, vertex4, color, duration);
            }

        }

        // origin�̉~��offset�̉~���Ȃ�
        Debug.DrawLine(origin, origin + offset, color, duration);
    }

    #endregion

    #region IsHit(bool��Ԃ��g�����\�b�h) static�N���X�ɂ��Ȃ��ƃ_���H

    // RaycastHit2D
    static public bool IsHit(this RaycastHit2D raycastHit2D)
    {
        return raycastHit2D;
    }

    // RaycastHit
    static public bool IsHit(this RaycastHit raycastHit)
    {
        return raycastHit.transform != null;
    }

    // --------------------------���s����--------------------------

    //static public implicit operator bool(RaycastHit raycastHit)
    //{
    //    bool answer;

    //    answer = raycastHit.transform != null;

    //    return answer;
    //}

    //static public bool operator ==(RaycastHit raycastHit, bool temp)
    //{
    //    bool answer;

    //    // ��v���Ă���true
    //    if((raycastHit.transform != null) == temp)
    //    {
    //        answer = true;
    //    }
    //    else
    //    {
    //        answer = false;
    //    }

    //    return answer;
    //}

    //static public bool operator !=(RaycastHit raycastHit, bool temp)
    //{
    //    bool answer;

    //    // ��v���ĂȂ�������true
    //    if ((raycastHit.transform != null) != temp)
    //    {
    //        answer = true;
    //    }
    //    else
    //    {
    //        answer = false;
    //    }

    //    return answer;
    //}

    //static public bool operator true(RaycastHit raycastHit)
    //{
    //    return raycastHit.transform != null;
    //}

    //static public bool operator false(RaycastHit raycastHit)
    //{
    //    return raycastHit.transform == null;
    //}

    #endregion

    #region Builder�p�^�[��

    // Builder�p�^�[��
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

    #endregion
}
