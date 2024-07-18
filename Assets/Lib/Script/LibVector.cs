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

        float raian = ChengeVectorToRadian_2D(x, y);
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

        float degree = ChengeVectorToDegree_2D(x, y);
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

    static public Vector3 ChengeVector3(this float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    //Trasnform

    //�w��̃|�C���g�܂Ŏ��ԊǗ��ňړ�����
    static public bool MoveFocusTime(this Transform transform, Vector3 goalPos, ref float time)
    {
        return transform.MoveFocusTime(goalPos, ref time, out Vector3 moveDir);
    }

    static public bool MoveFocusTime(this Transform transform, Vector3 goalPos, ref float time, out Vector3 moveDir)
    {
        if (time <= Time.deltaTime)
        {
            time = 0;
            moveDir = goalPos - transform.position;
            transform.position = goalPos;
            return true;
        }

        float rate = Time.deltaTime / time;
        Vector3 startPos = transform.position;
        transform.position = Vector3.Lerp(transform.position, goalPos, rate);
        moveDir = transform.position - startPos;
        time -= Time.deltaTime;
        return false;
    }

    //�w��̃|�C���g�܂ő��x�Ǘ��ňړ�����
    static public bool MoveFocusSpeed(this Transform transform, Vector3 goalPos, float speed)
    {
        return transform.MoveFocusSpeed(goalPos, speed, out Vector3 moveDir);
    }

    static public bool MoveFocusSpeed(this Transform transform, Vector3 goalPos, float speed, out Vector3 moveDir)
    {
        Vector3 startPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, goalPos, speed);
        moveDir = transform.position - startPos;

        return moveDir.magnitude < speed;
    }

    //�w��̕�������葬�x�ŐU�����
    static public void RotFocusSpeed(this Transform transform, Quaternion targetRotation, float speed)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed);
    }


    //Vecgtor3

    //Forward�x�N�g���i���ʁj�ɑ΂��ĕʂ̃x�N�g�����O��ǂ���ɂ��邩���肷��(1���O-1����j
    static public float VerticalElementOfForwardToDir(Vector3 forward, Vector3 dir)
    {
        float angle = Vector3.Angle(forward.normalized, dir.normalized);
        float vertical = (angle <= 90) ? 1.0f : -1.0f;
        return vertical;
    }

    //Forward�x�N�g���i���ʁj�ɑ΂��ĕʂ̃x�N�g�������E�ǂ���ɂ��邩���肷��(1���E-1�����j
    static public float HolizontalElementOfForwardToDir(Vector3 forward, Vector3 dir)
    {
        float holizontal = (Vector3.Cross(forward.normalized, dir.normalized).y < 0 ? -1 : 1);
        return holizontal;
    }

    //�x�N�g��������Y���p�x�ŉ�]������i�J�����̌������l���������͕����̌��m���j
    static public Vector3 RotationDirOfObjectFront(Transform obj, Vector3 dir)
    {
        Quaternion _targetRotation = Quaternion.AngleAxis(obj.eulerAngles.y, Vector3.up);
        Vector3 targetDirection = _targetRotation * dir.normalized;
        return targetDirection;
    }

    static public Vector3 RotationDirOfObjectFront(Transform obj, Vector2 dir)
    {
        return RotationDirOfObjectFront(obj, new Vector3(dir.x, 0, dir.y));
    }
}
