using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibBoxCollider
{
    // �e��BoxCollider�t���Ă�
    // �e��Transform�� Position, Rotation, Scale 0����Ȃ���OK
    // min, max �́@�e��Transform�� Position(0,0,0), Rotation(0,0,0), Scale(1,1,1) �̂Ƃ��̑��Έʒu

    /// <summary>
    /// ������localPosition��BoxCollider��ݒ肷��
    /// </summary>
    /// <param name="boxCollider"></param>
    /// <param name="min">�ŏ����_</param>
    /// <param name="max">�ő咸�_</param>
    /// <returns></returns>
    static public BoxCollider SetColliderAreaOfLocal(this BoxCollider boxCollider, Vector3 min, Vector3 max)
    {
        Vector3 midpoint = Vector3.Lerp(min, max, 0.5f);
        boxCollider.center = midpoint;
        boxCollider.size = max - min;

        return boxCollider;
    }
}
