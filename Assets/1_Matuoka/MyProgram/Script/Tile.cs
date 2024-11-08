using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn = false;
    private Material material;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        // �W�F�l�����R���C�_�[�̈����p�ϐ��Ɏ��g�̃f�[�^���i�[
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    public void ChangeIsOn()
    {
        isOn = !isOn;
    }

    public void ChangeMaterial(Color color)
    {
        material.color = color;
    }
}


/// <summary>
/// �^�C����������I�u�W�F�N�g�p�̃C���^�[�t�F�[�X
/// </summary>
public interface I_TileHit
{

}

/// <summary>
/// Tile���g���ꍇ�� GeneralCollider�̈���
/// </summary>
public class TilesGeneralColliderAttribute: GeneralColliderAttribute
{
    public Tile tile;

    public TilesGeneralColliderAttribute(Tile tile)
    {
        this.tile = tile;
    }
}