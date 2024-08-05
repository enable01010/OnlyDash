using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// �悭�g��Vector�֌W�̏����p�N���X
/// </summary>
public static class LibVector
{
    /// <summary>
    /// Transoform��X�̒l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="vector">���̃x�N�g��</param>
    /// <param name="value">�ύX��̒l</param>
    /// <returns></returns>
    static public Vector3 Set_X(Vector3 vector, float value)
    {
       
        vector.x = value;

        return vector;
    }

    /// <summary>
    /// Transoform��Y�̒l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="vector">���̃x�N�g��</param>
    /// <param name="value">�ύX��̒l</param>
    /// <returns></returns>
    static public Vector3 Set_Y(Vector3 vector, float value)
    {
        vector.y = value;

        return vector;
    }

    /// <summary>
    /// Transoform��Z�̒l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="vector">���̃x�N�g��</param>
    /// <param name="value">�ύX��̒l</param>
    /// <returns></returns>
    static public Vector3 Set_Z(Vector3 vector, float value)
    {
        vector.z = value;

        return vector;
    }

    /// <summary>
    /// Transoform��X�̒l�݂̂����Z���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_X(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.x += value;

        return answer;
    }

    /// <summary>
    /// Transoform��Y�̒l�݂̂����Z���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_Y(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.y += value;

        return answer;
    }

    /// <summary>
    /// Transoform��Z�̒l�݂̂����Z���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Vector3 Add_Z(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.z += value;

        return answer;
    }


    /// <summary>
    /// 2�����x�N�g�����p�x�i�ʓx�@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChangeVectorToRadian_2D(Vector2 vector)
    {
        float answer = 0;

        answer = Mathf.Atan2(vector.y, vector.x);

        return answer;
    }

    /// <summary>
    ///  2�����x�N�g�����p�x�i�ʓx�@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChangeVectorToRadian_2D(Vector3 vector)
    {
        float answer = 0;

        answer = Mathf.Atan2(vector.y, vector.x);

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x�i�ʓx�@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public float ChangeVectorToRadian_2D(float x, float y)
    {
        float answer = 0;

        answer = Mathf.Atan2(y, x);

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x�i�x���@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChangeVectorToDegree_2D(Vector2 vector)
    {
        float answer = 0;

        float raian = ChangeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2�����x�N�g�����p�x�i�x���@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChangeVectorToDegree_2D(Vector3 vector)
    {
        float answer = 0;

        float raian = ChangeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2�����x�N�g�����p�x�i�x���@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public float ChangeVectorToDegree_2D(float x, float y)
    {
        float answer = 0;

        float raian = ChangeVectorToRadian_2D(x, y);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChangeVectorToEulerAngle_2D(Vector2 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChangeVectorToDegree_2D(vector);
        answer = Set_Z(answer, degree);

        return answer;
    }


    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChangeVectorToEulerAngle_2D(Vector3 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChangeVectorToDegree_2D(vector);
        answer = Set_Z(answer, degree);

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChangeVectorToEulerAngle_2D(float x, float y)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChangeVectorToDegree_2D(x, y);
        answer = Set_Z(answer, degree);

        return answer;
    }


    // �g�����\�b�h

   

    static public Vector3 Only_X(this Vector3 vec)
    {
        vec.y = 0;
        vec.z = 0;
        return vec;
    }

    static public Vector3 Only_Y(this Vector3 vec)
    {
        vec.x = 0;
        vec.z = 0;
        return vec;
    }

    static public Vector3 Only_Z(this Vector3 vec)
    {
        vec.y = 0;
        vec.x = 0;
        return vec;
    }

    static public Vector3 Only_X(this float value)
    {
        return new Vector3(value, 0, 0);
    }

    static public Vector3 Only_Y(this float value)
    {
        return new Vector3(0, value, 0);
    }

    static public Vector3 Only_Z(this float value)
    {
        return new Vector3(0, 0, value);
    }

    static public float3 ChangeFloat3(this Vector3 vec)
    {
        return new float3(vec.x, vec.y, vec.z);
    }

    static public Vector3 ChangeVector3(this float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }
}
