using System.Collections;
using System.Collections.Generic;
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
    static public Vector3 Chenge_X(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.x = value;

        return answer;
    }

    /// <summary>
    /// Transoform��Y�̒l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="vector">���̃x�N�g��</param>
    /// <param name="value">�ύX��̒l</param>
    /// <returns></returns>
    static public Vector3 Chenge_Y(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.y = value;

        return answer;
    }

    /// <summary>
    /// Transoform��Z�̒l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="vector">���̃x�N�g��</param>
    /// <param name="value">�ύX��̒l</param>
    /// <returns></returns>
    static public Vector3 Chenge_Z(Vector3 vector, float value)
    {
        Vector3 answer = Vector3.zero;

        answer = vector;
        answer.z = value;

        return answer;
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
    static public float ChengeVectorToRadian_2D(Vector2 vector)
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
    static public float ChengeVectorToRadian_2D(Vector3 vector)
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
    static public float ChengeVectorToRadian_2D(float x, float y)
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
    static public float ChengeVectorToDegree_2D(Vector2 vector)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2�����x�N�g�����p�x�i�x���@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public float ChengeVectorToDegree_2D(Vector3 vector)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(vector);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    ///  2�����x�N�g�����p�x�i�x���@�j�ɕύX���鏈��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public float ChengeVectorToDegree_2D(float x, float y)
    {
        float answer = 0;

        float raian = ChengeVectorToRadian_2D(x,y);
        answer = raian * Mathf.Rad2Deg;

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(Vector2 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(vector);
        answer = Chenge_Z(answer, degree);

        return answer;
    }


    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(Vector3 vector)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(vector);
        answer = Chenge_Z(answer, degree);

        return answer;
    }

    /// <summary>
    /// 2�����x�N�g�����p�x (Vector3)�ɕύX���鏈��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    static public Vector3 ChengeVectorToEulerAngle_2D(float x, float y)
    {
        Vector3 answer = Vector3.zero;

        float degree = ChengeVectorToDegree_2D(x,y);
        answer = Chenge_Z(answer, degree);

        return answer;
    }


    // �g�����\�b�h

    /// <summary>
    /// Chenge_X �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Transform ChengeX_Position(this Transform transform, float value)
    {
        transform.position = Chenge_X(transform.position, value);

        return transform;
    }

    /// <summary>
    /// Chenge_Y �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Transform ChengeY_Position(this Transform transform, float value)
    {
        transform.position = Chenge_Y(transform.position, value);

        return transform;
    }

    /// <summary>
    /// Chenge_Z �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static public Transform ChengeZ_Position(this Transform transform, float value)
    {
        transform.position = Chenge_Z(transform.position, value);

        return transform;
    }

    /// <summary>
    /// Add_X �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    static public Transform AddX_Position(this Transform transform, float value)
    {
        transform.position = Add_X(transform.position, value);

        return transform;
    }

    /// <summary>
    /// Add_Y �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public Transform AddY_Position(this Transform transform, float value)
    {
        transform.position = Add_Y(transform.position, value);

        return transform;
    }

    /// <summary>
    /// Add_Z �̊g�����\�b�h
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    static public Transform AddZ_Position(this Transform transform, float value)
    {
        transform.position = Add_Z(transform.position, value);

        return transform;
    }
}
