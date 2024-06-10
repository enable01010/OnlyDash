using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �悭�g���F�֌W�̃N���X
/// </summary>
public class LibColor
{
    /// <summary>
    /// �F�̂P�l�݂̂�ύX���鏈��
    /// </summary>
    /// <param name="col">���̐F</param>
    /// <param name="value">�ύX��̒l(0~1�̏ꍇ�͂��̂܂܁j(1�`255�̏ꍇ�͔䗦�ɕϊ��j</param>
    /// <param name="mode">�ύX����F�i�����l�͓����x�j</param>
    /// <returns></returns>
    static public Color ChengeOneDir(Color color,float value,ColorT_Mode mode = ColorT_Mode.a)
    {
        Color answer = new Color();
        
        value = LibMath.CastLimit(value, 0, float.PositiveInfinity);
        value = (value <= 1.0f) ? value : LibMath.GetRangeToValue(value, 0, 255);
        answer = color;
        switch (mode)
        {
            case ColorT_Mode.r:
                answer.r = value;
                break;
            case ColorT_Mode.g:
                answer.g = value;
                break;
            case ColorT_Mode.b:
                answer.b = value;
                break;
            case ColorT_Mode.a:
                answer.a = value;
                break;
        }

        return answer;
    }

    /// <summary>
    /// �_�ł����鏈��
    /// </summary>
    /// <param name="col">���̐F</param>
    /// <param name="pitch">�_�ł̎���</param>
    /// <returns></returns>
    static public Color FlashColor(Color col,float pitch)
    {
        Color answer = new Color();

        float alpha = LibMath.GetMetoronomeZeroToOne(pitch);
        answer = ChengeOneDir(col, alpha);

        return answer;
    }


    /// <summary>
    /// Mathf.Lerp��Color��
    /// </summary>
    /// <param name="start"> �n�߂̐F </param>
    /// <param name="end"> �ڕW�̐F </param>
    /// <param name="ratio"> 0 �` 1 �Ŋ������� </param>
    /// <returns></returns>
    static public Color Lerp(Color start, Color end, float ratio)
    {
        Color answer;

        float r = Mathf.Lerp(start.r, end.r, ratio);
        float g = Mathf.Lerp(start.g, end.g, ratio);
        float b = Mathf.Lerp(start.b, end.b, ratio);
        float a = Mathf.Lerp(start.a, end.a, ratio);

        answer = new Color(r, g, b, a);

        return answer;
    }
}

public enum ColorT_Mode
{
    r,
    g,
    b,
    a
}
