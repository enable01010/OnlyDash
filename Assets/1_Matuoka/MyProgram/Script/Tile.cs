using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn { get; private set; } = false;
    private Material material;
    private ColorTiles owner;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        // �W�F�l�����R���C�_�[�̈����p�ϐ��Ɏ��g�̃f�[�^���i�[
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    /// <summary>
    /// ColorTiles������
    /// </summary>
    public void Init(ColorTiles owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// �؂�ւ��ĐF��ς���
    /// </summary>
    public void TileHit()
    {
        isOn = !isOn;

        if (isOn) ChangeColor(owner.TILE_COLOR_ON);
        else ChangeColor(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// ������Ԃɂ���
    /// </summary>
    /// <param name="temp"></param>
    public void SetFirstSetting()
    {
        isOn = false;

        if (isOn) ChangeColor(owner.TILE_COLOR_ON);
        else ChangeColor(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// �F��ς���
    /// </summary>
    /// <param name="color"></param>
    public void ChangeColor(Color color)
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