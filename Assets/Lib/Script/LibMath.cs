using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// �悭�g�����w�`�b�N�ȏ����p�̃N���X
/// </summary>
public class LibMath 
{

    /// <summary>
    /// ��������ɒl���������鏈��
    /// </summary>
    /// <param name="value">���ۂ̒l</param>
    /// <param name="min">����</param>
    /// <param name="max">���</param>
    /// <returns>���������l</returns>
    static public float CastLimit(float value, float min, float max)
    {
        float answer = 0;

        answer = value;
        answer = (answer < min) ? min : answer;
        answer = (answer > max) ? max : answer;

        return answer;
    }

    /// <summary>
    /// ��������ɑ΂����������䗦���v�Z���鏈��
    /// </summary>
    /// <param name="value">���ۂ̒l</param>
    /// <param name="min">����</param>
    /// <param name="max">���</param>
    /// <param name="isCast">0~1�ɋ������邩�ǂ���</param>
    /// <returns>�䗦</returns>
    static public float GetRangeToValue(float value,float min,float max,bool isCast = true)
    {
        float answer = 0;

        answer = (value - min) / (max - min);
        answer = (isCast) ? CastLimit(answer, 0, 1) : answer;

        return answer;
    }

    /// <summary>
    /// ��������ɑ΂���䗦����������v�Z���鏈��
    /// </summary>
    /// <param name="raito">�䗦</param>
    /// <param name="min">����</param>
    /// <param name="max">���</param>
    /// <param name="isCast">��������ɋ������邩�ǂ���</param>
    /// <returns>����</returns>
    static public float GetValueToRange(float raito,float min,float max,bool isCast = true)
    {
        float answer = 0;

        answer = min + (max - min) * raito;
        answer = (isCast) ? CastLimit(answer, min, max) : answer;

        return answer;
    }

    /// <summary>
    /// �䗦�𔽓]���鏈��
    /// </summary>
    /// <param name="raito">0�`1</param>
    /// <param name="isErrorLog">�G���[��ʒm���邩�i�f�t�H���g�͒ʒm����j</param>
    /// <returns>���]������</returns>
    static public float GetRaito_anti(float raito,bool isErrorLog = true)
    {
        if (isErrorLog == true &&�@(raito < 0 || raito > 1))
            Debug.LogWarning(
                "GetRaito_anti�֐��̈������s���̉\��������܂�\n" + 
                "�G���[��ʒm���Ȃ��ꍇ�͑�������false�ɂ��Ă�������"
                );

        float answer = 0;

        answer = 1 - answer;

        return answer;
    }

    /// <summary>
    /// ��������ɑ΂���䗦�̔��΂��v�Z���鏈��
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="min">����</param>
    /// <param name="max">���</param>
    /// <returns>�䗦�𔽓]������</returns>
    static public float GetRaito_anti(float value,float min,float max)
    {
        float answer = 0;

        answer = GetRangeToValue(value, min, max);
        answer = GetRaito_anti(answer);

        return answer;
    }

    /// <summary>
    /// �t�����v�Z���鏈��
    /// </summary>
    /// <param name="value">����</param>
    /// <returns>�t��</returns>
    static public float GetReverse(float value)
    {
        float answer = 0;

        answer = 1 / value;

        return answer;
    }

    /// <summary>
    /// 1�ȉ��̏����𔽓]���鏈��
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static public float OneMinus(float value, bool isErrorLog = true)
    {
        if (isErrorLog == true && value > 1)
            Debug.LogWarning(
                "OneMinus�֐��̈������s���̉\��������܂�\n" +
                "�G���[��ʒm���Ȃ��ꍇ�͑�������false�ɂ��Ă�������"
                );

        float answer = 0;

        answer = 1 - value;

        return answer;
    }

    /// <summary>
    /// 0�`1�܂ł̐��l�������I�ɕԂ��֐�
    /// </summary>
    /// <param name="pitch">����</param>
    /// <param name="offset">�����̂��炵</param>
    /// <returns></returns>
    static public float GetMetoronomeZeroToOne(float pitch,float offset = 0)
    {
        float answer = 0;

        float time = offset + Time.realtimeSinceStartup;//�Q�[�����J�n���Ă���̌o�ߎ���
        float theta = time * 2 * Mathf.PI * pitch;
        float y = Mathf.Sin(theta);
        answer = GetRangeToValue(y, -1, 1);

        return answer;
    }

    /// <summary>
    /// �p�x��0�`360�ɏC�����鏈��
    /// </summary>
    /// <param name="lfAngle"></param>
    /// <returns></returns>
    public static float ClampAngle(float lfAngle)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return lfAngle;
    }

    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    /// <summary>
    /// value���ō����x�܂ł̕ω��ʂŖڕW�l�ɋ߂Â���
    /// </summary>
    /// <param name="value"></param>
    /// <param name="goal"></param>
    /// <param name="maxSpeed"></param>
    /// <returns></returns>
    public static float MoveForcusSpeed(float value,float goal,float maxSpeed)
    {
        if (Mathf.Abs(value - goal) < maxSpeed) return goal;

        float direction = (value > goal) ? -1 : 1;
        return value + maxSpeed * direction;
    }
}
